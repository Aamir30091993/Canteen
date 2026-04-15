using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using GCC_Canteen.ViewModel;

namespace GCC_Canteen.Models.Repository
{
    public class WebGridDataLayer
    {

        public GridSettingData GridData(int pageID, string Control)
        {

            UserInfo userInfo = HttpContext.Current.Session["UserInfo"] as UserInfo;
            using (var db = new MMModel())
            {
                // If using Code First we need to make sure the model is built before we open the connection 
                // This isn't required for models created with the EF Designer 
                db.Database.Initialize(force: false);

                // Create a SQL command to execute the sproc 
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "[spGetSearchControls]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter paramPageID = new SqlParameter("@PageID", System.Data.SqlDbType.Int);
                paramPageID.Value = Convert.ToInt32(pageID);
                cmd.Parameters.Add(paramPageID);

                SqlParameter paramControl = new SqlParameter("@Control", System.Data.SqlDbType.VarChar, 50);
                paramControl.Value = Control.ToString();
                cmd.Parameters.Add(paramControl);

                SqlParameter paramUserID = new SqlParameter("@UserID", System.Data.SqlDbType.Int);
                paramUserID.Value = userInfo.UserID;
                cmd.Parameters.Add(paramUserID);
                
                try
                {

                    db.Database.Connection.Open();
                    // Run the sproc  
                    var reader = cmd.ExecuteReader();

                    // Read Blogs from the first result set 
                    var gridSettings = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<GridSettings>(reader).AsEnumerable().ToList();


                     ////Move to second result set and read Posts BranchSystemDetails
                    reader.NextResult();
                    var gridColumnSetting = ((IObjectContextAdapter)db)
                        .ObjectContext
                        .Translate<GridColumnSetting>(reader).AsEnumerable().ToList();
                    
                    reader.NextResult();
                    reader.NextResult();
                    reader.NextResult();
                    reader.NextResult();
                    List<string> gridHeaderTextStyle = ((IObjectContextAdapter)db)
                    .ObjectContext
                    .Translate<string>(reader).AsEnumerable().ToList();

                    //reader.NextResult();
                    //var branchList = ((IObjectContextAdapter)db)
                    //    .ObjectContext
                    //    .Translate<BranchListModel>(reader).AsEnumerable().ToList();

                    GridSettingData ObjGridData = new GridSettingData();
                    ObjGridData.WebGridSetting = gridSettings[0];
                    ObjGridData.WebGridColumnSetting = gridColumnSetting;
                    ObjGridData.HeaderTextStyle = gridHeaderTextStyle;
                    return ObjGridData;
                }
                finally
                {
                    db.Database.Connection.Close();
                }
            }
        }
    }
}