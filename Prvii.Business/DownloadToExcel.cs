using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prvii.Entities;
using Prvii.Entities.DataEntities;
using Prvii.Entities.Enumerations;
using System.Web;
namespace Prvii.Business
{
  public  class DownloadToExcel
    {


      public static string GenerateLogFile(long channelID, bool Preclude)
      {
          int i = 1;
          //var reportList = UserProfileManager.GetUsers();
          var reportList = ChannelSubscribersManager.GetSubscribersDownload(channelID, Preclude);
          StringBuilder csvExport = new StringBuilder();

          csvExport.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\"",
                 "SLNO", "First Name", "Last Name", "Nickname", "Email", "Zip Code", "Mobile", "Telephone", "Address1", "Address2", "City", "State Name", "Country Name", "TimeZone", "IsActive", "Celebrity"));

          foreach (var item in reportList)
          {
              csvExport.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\"",
                  i,
                  item.Firstname, item.Lastname, item.Nickname, item.Email, item.ZipCode, item.Mobile, item.Telephone, item.Address1, item.Address2, item.City, item.StateName, item.CountryName, item.TimeZoneID, item.IsActive, item.Celebrity));               
              i++;
          }
          return csvExport.ToString();
      }




      public static void GetAllSubstciberDetails()
      {
          try
          {
              string csvExportContents = GenerateLogFile(0, true);
              byte[] data = ASCIIEncoding.ASCII.GetBytes(csvExportContents);             
              HttpContext.Current.Response.Clear();
              HttpContext.Current.Response.ContentType = "APPLICATION/OCTET-STREAM";
              HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=Prvii.csv");
              HttpContext.Current.Response.OutputStream.Write(data, 0, data.Length);
              HttpContext.Current.Response.End();
              
              // HttpContext.Current.ApplicationInstance.CompleteRequest();
          }
          catch (Exception ex)
          {
              throw ex;
          }

      }

      public static bool GetPrecludeCelebrity(long channelID)
      {
          using (PrviiEntities context = new Entities.PrviiEntities())
          {             
                  return context.Channels.Where(w => w.ID == channelID && w.Preclude == true).Any();
          }
      }

      
    }


}
