using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prvii.Entities;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using Prvii.Entities.Enumerations;
using Prvii.Entities.DataEntities;

namespace Prvii.Business
{
    public class UserProfileManager
    {
        public static UserProfile Authenticate(string userId, string password)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.UserProfiles.FirstOrDefault(e => e.Email == userId && e.Password == password && e.IsActive == true);
            }
        }

        public static UserProfile GetByID(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.UserProfiles.FirstOrDefault(x => x.ID == id);
            }
        }

        public static UserProfile GetByEmail(string email)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.UserProfiles.FirstOrDefault(x => x.Email == email);
            }
        }

        public static List<UserProfileData> GetUsers()
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var query = from u in context.UserProfiles
                            join ut in context.UserRoles on u.ID equals ut.UserID
                            select new
                            {
                                u.ID,
                                u.Firstname,
                                u.Lastname,
                                u.Email,
                                u.ZipCode,
                                u.IsActive,
                                u.Mobile,
                                UserRole = ut.RoleID
                            };

                return query.ToList().Select(x => new UserProfileData
                {
                    ID = x.ID,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname,
                    Email = x.Email,
                    ZipCode = x.ZipCode,
                    IsActive = x.IsActive,
                    Mobile = x.Mobile,
                    RoleName = ((Role)x.UserRole).ToString()
                }).OrderBy(o => o.Name).ThenBy(o => o.RoleName).ToList();
            }
        }

        public static List<UserProfileData> GetUsersNew()
        {
            List<UserProfileData> uprofilelist = new List<UserProfileData>();
           
           
            using (PrviiEntities context = new PrviiEntities())
            {
                var uProfiles = context.UserProfiles.ToList();
                foreach (var item in uProfiles)
                {
                    UserProfileData userprofile = new UserProfileData();
                    userprofile.ID = item.ID;
                    userprofile.Firstname = item.Firstname;
                    userprofile.Lastname = item.Lastname;
                    userprofile.Email = item.Email;
                    userprofile.ZipCode = item.ZipCode;
                    userprofile.IsActive = item.IsActive;
                    userprofile.Mobile = item.Mobile;
                    var rolList = context.UserRoles.Where(a => a.UserID == item.ID).ToList();
                    foreach (var role in rolList)
                    {
                        userprofile.RoleName += ((Role)role.RoleID).ToString() + ",";
                    }
                    userprofile.RoleName = userprofile.RoleName.Substring(0, userprofile.RoleName.Length - 1);
                    uprofilelist.Add(userprofile);
                }
               

            }
            return uprofilelist.OrderBy(o => o.Name).ToList();
        }

        public static bool Exists(string emailID, long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.UserProfiles.Any(x => x.Email.ToLower() == emailID && x.ID != id);
            }
        }

        public static List<short> GetRoleIDList(long userID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.UserRoles.Where(p => p.UserID == userID).Select(x => x.RoleID).ToList();
            }
        }

        public static List<UserRole> GetRoles(long userID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.UserRoles.Where(p => p.UserID == userID).ToList();
            }
        }

        public static void Save(UserProfile user, List<UserRole> userRoles)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                context.Entry(user).State = user.ID == 0 ? EntityState.Added : EntityState.Modified;

                List<UserRole> rolesToRemove = new List<UserRole>();

                foreach (var role in user.UserRoles)
                {
                    if (!userRoles.Any(x => x.RoleID == role.RoleID))
                    {
                        rolesToRemove.Add(role);
                    }
                }

                rolesToRemove.ForEach(x => context.UserRoles.Remove(x));

                foreach (var role in userRoles)
                {
                    if (!user.UserRoles.Any(x => x.RoleID == role.RoleID))
                    {
                        user.UserRoles.Add(role);
                    }
                }

                context.SaveChanges();
            }
        }

        public static void ForgotPassword(UserProfile user)
        {
            string emailBody = EMailTemplateManager.GetForgotPasswordEmailBody(user);

            EMailManager emailer = new EMailManager();
            emailer.MailTo = user.Email;
            emailer.Subject = "Prvii App Reset Password";
            emailer.MailBody = emailBody;
            emailer.SendMail();
        }

        public static void PasswordReset(long id, string password)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var user = context.UserProfiles.FirstOrDefault(x => x.ID == id);
                user.Password = password;
                context.SaveChanges();
            }
        }

        //public static List<UserRole> GetUserProfileTypeByUserID(long UserID)
        //{
        //    using (PrviiEntities context = new PrviiEntities())
        //    {
        //        return context.UserRoles.Where(p => p.UserID == UserID).ToList();
        //    }
        //}

        public static bool IsAuthenticateUser(long UserId)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var userDetails = context.UserProfiles.FirstOrDefault(e => e.ID == UserId && e.IsActive == true);
                bool status = userDetails != null ? true : false;
                return status;
            }
        }

        public static int changePassword(long UserID,string OldPassword,string NewPassword)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                try 
                {
                    UserProfile userProfile = context.UserProfiles.Where(u => u.ID == UserID && u.Password == OldPassword).FirstOrDefault();
                    if (userProfile != null)
                    {
                        userProfile.Password = NewPassword;
                        context.UserProfiles.Attach(userProfile);
                        context.Entry(userProfile).State = EntityState.Modified;
                        context.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch (Exception ex)
                {

                    return -2;
                }
            }
        }

        public static long ManageUserMaster(UserProfile userProfile)
        {
            using (PrviiEntities context = new PrviiEntities())
            {

                context.UserProfiles.Attach(userProfile);
                context.Entry(userProfile).State = userProfile.ID == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();
                return userProfile.ID;
            }
        }

       

        public static string GetCountryName(long id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.Countries.Where(w => w.ID == id).FirstOrDefault().Name;
            }
        }
        public static string GetStateName(long? id)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                if (id != null)
                    return context.States.Where(w => w.ID == id).FirstOrDefault().Name;
                else
                    return "";
            }
        }

        public static string GetSubscriberTimeZone(long id)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                return context.UserProfiles.Where(x => x.ID == id).Select(x => x.TimeZoneID).FirstOrDefault();
            }
        }

        public static string GetSubscriberDeliveryMechanisms(long id)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                var query = from cs in context.ChannelSubscribers
                            join u in context.UserProfiles on cs.SubscriberID equals u.ID
                            where cs.ID == id && cs.IsActive
                            select new
                            {
                                DeliveryMechanisms = u.DeliveryMechanisms
                            };


                return query.Select(q => q.DeliveryMechanisms).FirstOrDefault();
            }
        }

        public static string GetSubscriberNameByChannelSubscriberID(long id)
        {
            using (PrviiEntities context = new Entities.PrviiEntities())
            {
                var query = from cs in context.ChannelSubscribers
                            join u in context.UserProfiles on cs.SubscriberID equals u.ID
                            where cs.ID == id && cs.IsActive
                            select new
                            {
                                Name = u.Firstname + " " + u.Lastname
                            };


                return query.Select(q => q.Name).FirstOrDefault();
            }
        }


        public static bool SaveUserDeviceInfo(UserDeviceInfo userDevice)
        {
            using (PrviiEntities context = new PrviiEntities())
            {

                var deviceInfo = context.UserDeviceInfoes.Where(p => p.SubscriberId == userDevice.SubscriberId && p.DeviceId == userDevice.DeviceId && p.IsActive == true).FirstOrDefault();
                
                if (deviceInfo == null)
                {
                    context.Entry(userDevice).State = userDevice.Id == 0 ? EntityState.Added : EntityState.Modified;
                }              

                context.SaveChanges();
                return true;
            }
        }


        public static List<UserDeviceInfo> GetUserDeviceInfoByUserID(long userID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.UserDeviceInfoes.Where(p => p.SubscriberId == userID).ToList();
               
            }
        }

        public static UserDeviceInfo GetUserDeviceInfoByDeviceID(string DeviceID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                return context.UserDeviceInfoes.Where(p => p.DeviceId == DeviceID && p.IsActive == true).FirstOrDefault();

            }
        }

        public static UserProfile GetUserByDeviceID(string DeviceID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
               // var UserID= context.UserDeviceInfoes.Where(p => p.DeviceId == DeviceID && p.IsActive == true).FirstOrDefault().Id;
               // return context.UserProfiles.FirstOrDefault(x => x.ID == UserID);
                return context.UserDeviceInfoes.Include("UserProfiles").Where(w => w.DeviceId == DeviceID && w.IsActive == true).Select(s => s.UserProfile).FirstOrDefault();
            }
        }


        public static bool LogoutUserDevice(long userID,string deviceID)
        {
            using (PrviiEntities context = new PrviiEntities())
            {
                var deviceInfo = context.UserDeviceInfoes.Where(p => p.SubscriberId == userID && p.DeviceId == deviceID && p.IsActive == true).FirstOrDefault();
                deviceInfo.IsActive = false;
                deviceInfo.ModifiedDate = DateTime.UtcNow;
                context.Entry(deviceInfo).State =  EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }


        public static bool SaveUserDeviceToken(long userID, string deviceID,string DeviceToken)
        {
            using (PrviiEntities context = new PrviiEntities())
            {

                var deviceInfo = context.UserDeviceInfoes.Where(p => p.SubscriberId == userID && p.DeviceId == deviceID && p.IsActive == true).FirstOrDefault();
                deviceInfo.DeviceToken = DeviceToken;
                deviceInfo.ModifiedDate = DateTime.UtcNow;
                context.Entry(deviceInfo).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

    }
}
