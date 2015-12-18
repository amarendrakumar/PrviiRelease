using Prvii.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Business
{
    public class MasterManager
    {
        public static IList GetAllCountry()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Countries.Where(w => w.IsActive == true).OrderBy(o => o.Name)
                     .Select(c => new
                     {
                         ID = c.ID,
                         Name = c.Name,
                         IsActive = c.IsActive
                     }).ToList();

            }
        }

        public static List<State> GetState(long countryID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.States.Where(w => w.CountryID == countryID && w.IsActive == true)
                    .OrderBy(o => o.Name).ToList();

            }
        }

        public static IList GetStateByCountryID(long countryID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.States.Where(w => w.CountryID==countryID && w.IsActive == true).OrderBy(o => o.Name)
                     .Select(c => new
                     {
                         ID = c.ID,
                         Name = c.Name,
                         IsActive = c.IsActive
                     }).ToList();

            }
        }

       

    }
}
