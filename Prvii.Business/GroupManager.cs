using Prvii.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Business
{
    public class GroupManager
    {
        public static IList GetAll()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Groups.Select(x => new
                    {
                        x.ID,
                        x.Name,
                        x.Mobile,
                        x.Email,
                        x.ZipCode,
                        x.IsActive,
                        CelebrityCount = x.GroupChannels.Count,
                        UserCount = x.UserProfiles.Count
                    }).ToList();
            }
        }

        public static Group GetByID(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Groups.FirstOrDefault(x=> x.ID == id);
            }
        }

        public static string GetName(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Groups.Where(x => x.ID == id).Select(x=>x.Name).FirstOrDefault();
            }
        }

        public static bool Exists(string name, long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Groups.Any(x => x.Name.ToLower() == name.ToLower() && x.ID != id);
            }
        }

        public static void Save(Group group)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                context.Entry(group).State = group.ID != 0 ? EntityState.Modified : EntityState.Added;
                context.SaveChanges();
            }
        }

        public static void Delete(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var group = context.Groups.Include(x => x.GroupChannels).FirstOrDefault();

                context.GroupChannels.RemoveRange(group.GroupChannels);
                context.Groups.Remove(group);
                context.SaveChanges();
            }
        }

        public static List<GroupChannel> GetChannelsByGroup(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.GroupChannels.Where(x => x.GroupID == id).ToList();
            }
        }

        public static void SaveChannels(long groupID, List<long> channelIDs)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var existingList = context.GroupChannels.Where(x => x.GroupID == groupID).ToList();

                foreach(var item in existingList)
                {
                    if (!channelIDs.Contains(item.ID))
                    {
                        context.GroupChannels.Remove(item);
                    }
                }

                foreach(var item in channelIDs)
                {
                    if(!existingList.Any(x=>x.ID == item))
                    {
                        var newItem = new GroupChannel
                        {
                            GroupID = groupID,
                             ChannelID = item,
                             EFD = DateTime.Today
                        };

                        context.Entry(newItem).State = EntityState.Added;
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
