﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Entities;
using Core.Services;
using NHibernate.Linq;

namespace Data.Services
{
    public class DiuretikService : Service<Diuretik>, IDiuretikService
    {
        public DiuretikService(NewDataLayer dataLayer) : base(dataLayer)
        {
        }

        public override Diuretik Get(int id)
        {
            using (var session = DataLayer.GetSession())
            {
                var obj = session.Get<Diuretik>(id);

                if (obj == null) return null;

                obj.Pakovanje = session.Get<Pakovanje>(obj.Pakovanje?.Id);

                obj.BolestList = session.Query<Bolest>().Where(x => x.Deleted == false).ToList();
                obj.KontraindikacijaList =
                    session.Query<Kontraindikacija>().Where(x => x.Deleted == false).ToList();
                obj.ProdajnoMestoList = session.Query<ProdajnoMesto>().Where(x => x.Deleted == false).ToList();
                obj.ReceptList = session.Query<Recept>().Where(x => x.Deleted == false).ToList();
                obj.ProizvodjacList = session.Query<Proizvodjac>().Where(x => x.Deleted == false).ToList();

                return obj;
            }
        }

        public DataTable GetDataTable(bool naRecept)
        {
            var dataTable = new DataTable("Diuretik");

            dataTable.Columns.Add("id");
            dataTable.Columns.Add("Tip Leka");
            dataTable.Columns.Add("Participacija");
            dataTable.Columns.Add("Cena");
            dataTable.Columns.Add("HemijskiNazivLeka");
            dataTable.Columns.Add("NacainDoziranja");
            dataTable.Columns.Add("Recept");
            //dataTable.Columns.Add(Constants.ConcatenatedField, typeof(string), "Id + ' : ' +HemijskiNazivLeka");

            List<Diuretik> leks;
            using (var session = DataLayer.GetSession())
            {
                leks = session.QueryOver<Diuretik>()
                    .Where(x => x.NaRecept == naRecept && x.Deleted == false).List<Diuretik>() as List<Diuretik>;
            }
            if (leks == null) return dataTable;
            foreach (var lek in leks)
            {
                dataTable.Rows.Add(
                    lek.Id,
                    lek.TipLeka.ToString(),
                    lek.ProcenatParticipacije,
                    lek.Cena,
                    lek.NazivLeka.HemijskiNaziv,
                    lek.NacinDoziranja.ToString(),
                    lek.NaRecept ? "Da" : "Ne"
                    );
            }
            return dataTable;
        }
    }
}