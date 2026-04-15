using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using GCC_Canteen.Models;

namespace GCC_Canteen.App_Code
{
    public class Mail
    {
        public static bool SendMail(int ActionID, int TemplateID, string spNmame, int RecordID, string KeyValue, int UserID, string Password = "")
        {
            MailMessage message = new MailMessage();
            bool flag = false;
            MMModel db = new MMModel();
            SqlConnection MyConn = new SqlConnection(db.Database.Connection.ConnectionString);
            try
            {
                DataSet dsMailConfigData = new DataSet();
                SqlParameter param = new SqlParameter();
                if (MyConn.State == ConnectionState.Closed)
                {
                    MyConn.Open();
                }
                SqlCommand cmd = new SqlCommand(spNmame, MyConn);
                cmd.CommandType = CommandType.StoredProcedure;
                param = cmd.Parameters.Add("@ActionID", SqlDbType.Int);
                param.Value = ActionID;
                param = cmd.Parameters.Add("@TemplateID", SqlDbType.Int);
                param.Value = TemplateID;
                param = cmd.Parameters.Add("@RecordID", SqlDbType.Int);
                param.Value = RecordID;
                param = cmd.Parameters.Add("@UserID", SqlDbType.Int);
                param.Value = UserID != 0 ? UserID : Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                param = cmd.Parameters.Add("@KeyValue", SqlDbType.VarChar);
                param.Value = KeyValue;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                cmd.CommandTimeout = 600000000;
                da.Fill(dsMailConfigData);
                if (MyConn.State == ConnectionState.Open)
                    MyConn.Close();
                cmd.Dispose();
                MyConn.Dispose();

                DataTable MessageData = dsMailConfigData.Tables[1];
                if (MessageData.Rows.Count > 0)
                {
                    string from;
                    if (dsMailConfigData.Tables[2].Columns.Contains("FromAccount"))
                    {
                        if (dsMailConfigData.Tables[2].Rows[0]["FromAccount"].ToString() == "")
                            from = MessageData.Rows[0]["FromAccount"].ToString().Replace(";", "").ToString();
                        else
                            from = dsMailConfigData.Tables[2].Rows[0]["FromAccount"].ToString().Replace(";", "").ToString();
                    }
                    else
                        from = MessageData.Rows[0]["FromAccount"].ToString().Replace(";", "").ToString();

                    //string from = MessageData.Rows[0]["FromAccount"].ToString();
                    message.From = new MailAddress(from);
                    string To = MessageData.Rows[0]["ToAccount"].ToString() + ';' + dsMailConfigData.Tables[2].Rows[0]["ToAccount"].ToString();
                    string Cc = MessageData.Rows[0]["CCAccount"].ToString() + ';' + dsMailConfigData.Tables[2].Rows[0]["CCAccount"].ToString();
                    string Bcc = MessageData.Rows[0]["BCCAccount"].ToString() + ';' + dsMailConfigData.Tables[2].Rows[0]["BCCAccount"].ToString();
                    if (Password != "")
                    {
                        dsMailConfigData.Tables[2].Rows[0]["Password"] = Password;
                    }
                    string[] strTo = To.Split(';');
                    string[] strCc = Cc.Split(';');
                    string[] strBcc = Bcc.Split(';');

                    if (strTo != null)
                        for (int i = 0; i < strTo.Length; i++)
                        {
                            if (strTo[i].Trim() != string.Empty)
                            {
                                if (!message.To.ToString().Contains(strTo[i]))
                                    message.To.Add(strTo[i]);
                            }
                        }

                    if (strCc != null)
                        for (int i = 0; i < strCc.Length; i++)
                        {
                            if (strCc[i].Trim() != string.Empty)
                            {
                                if (!message.CC.ToString().Contains(strCc[i]))
                                    message.CC.Add(strCc[i]);
                            }
                        }

                    if (strBcc != null)
                        for (int i = 0; i < strBcc.Length; i++)
                        {
                            if (strBcc[i].Trim() != string.Empty)
                            {
                                if (!message.Bcc.ToString().Contains(strBcc[i]))
                                    message.Bcc.Add(strBcc[i]);
                            }
                        }

                    string subject = "";
                    if (dsMailConfigData.Tables[2].Columns.Contains("EmailSubject"))//(TemplateID == 13 || TemplateID == 14 || TemplateID == 92 || TemplateID == 93)
                    {
                        subject = dsMailConfigData.Tables[2].Rows[0]["EmailSubject"].ToString();
                        for (int i = 0; i < dsMailConfigData.Tables[2].Columns.Count; i++)
                        {
                            subject = subject.Replace("#[" + dsMailConfigData.Tables[2].Columns[i].ColumnName + "]#", Convert.ToString(dsMailConfigData.Tables[2].Rows[0][i]));
                            subject = subject.Replace("#" + dsMailConfigData.Tables[2].Columns[i].ColumnName + "#", Convert.ToString(dsMailConfigData.Tables[2].Rows[0][i]));
                        }
                    }
                    else
                    {
                        subject = MessageData.Rows[0]["EmailSubject"].ToString();
                    }

                    string templatepath = HttpContext.Current.Request.PhysicalApplicationPath + "Template\\" + MessageData.Rows[0]["TemplatePath"].ToString();
                    string body = Mail.ComposeMailBody(templatepath, dsMailConfigData);

                    if (dsMailConfigData.Tables.Count >= 4)
                    {
                        foreach (DataRow row in dsMailConfigData.Tables[3].Rows)
                        {
                            Attachment attach = new Attachment(row["AttachmentPath"].ToString());
                            attach.Name = row["AttachmentName"].ToString();
                            message.Attachments.Add(attach);
                        }
                    }


                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    DataTable smptdata = dsMailConfigData.Tables[0];
                    SmtpClient mailClient = new SmtpClient();
                    mailClient.Host = smptdata.Rows[0]["Host"].ToString();
                    mailClient.Port = Convert.ToInt32(smptdata.Rows[0]["Port"]);
                    string UserName = smptdata.Rows[0]["UserName"].ToString();
                    string password = smptdata.Rows[0]["Password"].ToString();
                    mailClient.Credentials = new System.Net.NetworkCredential(UserName, password);
                    if (Convert.ToBoolean(smptdata.Rows[0]["SSL"]) == true)
                        mailClient.EnableSsl = true;
                    else
                        mailClient.EnableSsl = false;

                    mailClient.Send(message);
                    flag = true;
                    if (flag)
                    {
                        if (flag)
                        {
                            MMModel obj_db = new MMModel();
                            SqlConnection SqlConn = new SqlConnection(obj_db.Database.Connection.ConnectionString);
                            if (SqlConn.State == ConnectionState.Closed)
                            {
                                SqlConn.Open();
                            }
                            SqlTransaction Sqltran = SqlConn.BeginTransaction();
                            try
                            {
                                SqlParameter SqlParam;


                                SqlCommand cmdParam = new SqlCommand("spUpdateEmailQueue", SqlConn, Sqltran);
                                cmdParam.CommandType = CommandType.StoredProcedure;
                                SqlParam = cmdParam.Parameters.Add("@EmailQueueID", SqlDbType.Int);
                                SqlParam.Value = Convert.ToInt32(MessageData.Rows[0]["EmailQueueID"]);
                                SqlParam = cmdParam.Parameters.Add("@TemplateID", SqlDbType.Int);
                                SqlParam.Value = TemplateID;
                                SqlParam = cmdParam.Parameters.Add("@ToEmailIDs", SqlDbType.VarChar, 500);
                                SqlParam.Value = To;
                                SqlParam = cmdParam.Parameters.Add("@CCEmailIDs", SqlDbType.VarChar, 500);
                                SqlParam.Value = Cc;
                                SqlParam = cmdParam.Parameters.Add("@BCCEmailIDs", SqlDbType.VarChar, 500);
                                SqlParam.Value = Bcc;
                                SqlParam = cmdParam.Parameters.Add("@Body", SqlDbType.VarChar, 8000);
                                SqlParam.Value = body;
                                cmdParam.CommandTimeout = 60000;
                                cmdParam.ExecuteNonQuery();
                                Sqltran.Commit();
                            }
                            catch
                            {
                                Sqltran.Rollback();
                                return flag;

                            }
                            finally
                            {

                            }
                        }

                    }
                }
                else
                {
                    flag = false;
                }
                return flag;

            }
            catch
            {
                return flag;
            }
            finally
            {

                message.Dispose();
            }

        }


        public static string ComposeMailBody(string templetePath, DataSet mailData)
        {
            StreamReader rdr = new StreamReader(templetePath);
            string content = rdr.ReadToEnd();
            rdr.Close();
            rdr.Dispose();

            if (mailData.Tables[2].Rows.Count == 1)
            {
                for (int i = 0; i < mailData.Tables[2].Columns.Count; i++)
                {
                    content = content.Replace("#" + mailData.Tables[2].Columns[i].ColumnName + "#", Convert.ToString(mailData.Tables[2].Rows[0][i]));
                }
            }
            if (mailData.Tables[1].Rows.Count == 1)
            {
                for (int i = 0; i < mailData.Tables[1].Columns.Count; i++)
                {
                    content = content.Replace("#" + mailData.Tables[1].Columns[i].ColumnName + "#", Convert.ToString(mailData.Tables[1].Rows[0][i]));
                }
            }
            return content;

        }

        //private static Attachments Attachment()
        //{
        //    Attachments attachment = new Attachments();
        //    attachment.Add("~/Images/ColliersLM_logo.gif", "image/gif", "Logo", true);

        //    return attachment;
        //}

    }
}