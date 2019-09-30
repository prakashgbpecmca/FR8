using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;

namespace Frayte.Services.Utility
{
    public static class FrayteEmail
    {

        static FrayteEntities dbContext = new FrayteEntities();

        public static void SendFrayteEmail(string toEmail, string ccEmail, string DisplayName, string subject, string mailContent, string Attachmentfilepath, List<string> inlineImages, string Status, int customerId)
        {
            try
            {
                var customerCompanyDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == customerId).FirstOrDefault();
                
                using (MailMessage mail = new MailMessage())
                {
                    string host = string.Empty;
                    int port = 0;
                    string userName = string.Empty;
                    string password = string.Empty;
                    string enableSsl = string.Empty;

                    if (customerCompanyDetail != null)
                    {
                        //HOST
                        host = customerCompanyDetail.SMTPHostName;

                        //PORT
                        port = Convert.ToInt32(customerCompanyDetail.SMTPport);

                        //USER NAME
                        userName = customerCompanyDetail.SMTPUserName;

                        //PASSWORD
                        password = customerCompanyDetail.SMTPPassword;

                        //Enable Ssl
                        enableSsl = customerCompanyDetail.SMTPEnableSsl;

                        //FROM MAIL
                        string fromMail = customerCompanyDetail.SMTPFromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, customerCompanyDetail.SMTPDisplayName);
                        mail.From = MailFrom;
                    }
                    else
                    {
                        //HOST
                        host = AppSettings.MailHostName;

                        //PORT
                        port = Convert.ToInt32(AppSettings.SMTPport);

                        //USER NAME
                        userName = AppSettings.SMTPUserName;

                        //PASSWORD
                        password = AppSettings.SMTPPassword;

                        //Enable Ssl
                        enableSsl = AppSettings.EnableSsl;

                        //FROM MAIL
                        string fromMail = AppSettings.FromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, DisplayName);
                        mail.From = MailFrom;
                    }

                    //BCC
                    string bccEmail = AppSettings.BCC;

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    if (!string.IsNullOrEmpty(ccEmail))
                    {
                        string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                        string[] DistiCC = CCarray.Distinct().ToArray();
                        for (int i = 0; i < DistiCC.Length; i++)
                        {
                            strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                        }

                        //CC
                        string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                        for (int i = 0; i < strCCAdress.Length; i++)
                        {
                            if (Convert.ToString(strCCAdress[i]).Trim() != "")
                            {
                                MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                                mail.CC.Add(MailCC);
                            }
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            attachment.Name = System.IO.Path.GetFileName(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);

                            //System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            //mail.Attachments.Add(attachment);
                        }
                    }

                    if (inlineImages.Count > 0)
                    {
                        foreach (string imagePath in inlineImages)
                        {
                            if (Status == "Confirmation")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("TrackShipment.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "TrackShipment";
                                }
                                else
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                            }
                            else if (Status == "Cancel")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("Confirm.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Confirm";
                                }
                                else if (imagePath.Contains("Reject.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Reject";
                                }
                            }
                            else if (Status == "FuelCharge")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("FuelSurCharge.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FuelSurCharge";
                                }
                            }
                            else if (Status == "ExchangeRate")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("ExchangeRate.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "ExchangeRate";
                                }
                            }
                            else if (Status == "Amend")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("Confirm.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Confirm";
                                }
                                else if (imagePath.Contains("Reject.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Reject";
                                }
                                if (imagePath.Contains("Amend.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Amend";
                                }
                            }
                            //For Special Customer
                            else
                            {
                                Attachment logoImageAtt = new Attachment(imagePath);
                                mail.Attachments.Add(logoImageAtt);
                                logoImageAtt.ContentDisposition.Inline = true;
                                logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                logoImageAtt.ContentId = "FrayteLogo";
                            }
                        }
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendFrayteEmail(string toEmail, string ccEmail, string subject, string mailContent, string Attachmentfilepath, string headerLogo, int customerId)
        {
            try
            {
                var customerCompanyDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == customerId).FirstOrDefault();

                using (MailMessage mail = new MailMessage())
                {
                    string host = string.Empty;
                    int port = 0;
                    string userName = string.Empty;
                    string password = string.Empty;
                    string enableSsl = string.Empty;

                    if (customerCompanyDetail != null)
                    {
                        host = customerCompanyDetail.SMTPHostName;

                        //PORT
                        port = Convert.ToInt32(customerCompanyDetail.SMTPport);

                        //USER NAME
                        userName = customerCompanyDetail.SMTPUserName;

                        //PASSWORD
                        password = customerCompanyDetail.SMTPPassword;

                        //Enable Ssl
                        enableSsl = customerCompanyDetail.SMTPEnableSsl;

                        //FROM MAIL
                        string fromMail = customerCompanyDetail.SMTPFromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, customerCompanyDetail.SMTPDisplayName);
                        mail.From = MailFrom;
                    }
                    else
                    {
                        host = AppSettings.MailHostName;

                        //PORT
                        port = Convert.ToInt32(AppSettings.SMTPport);

                        //USER NAME
                        userName = AppSettings.SMTPUserName;

                        //PASSWORD
                        password = AppSettings.SMTPPassword;

                        //Enable Ssl
                        enableSsl = AppSettings.EnableSsl;


                        //FROM MAIL
                        string fromMail = AppSettings.FromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, AppSettings.DisplayName);
                        mail.From = MailFrom;
                    }
                    //HOST


                    //BCC
                    string bccEmail = AppSettings.BCC;


                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (!string.IsNullOrEmpty(headerLogo))
                    {
                        Attachment logoImageAtt = new Attachment(headerLogo);
                        mail.Attachments.Add(logoImageAtt);
                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "FrayteLogo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail(string toEmail, string ccEmail, string subject, string mailContent, string Attachmentfilepath, string headerLogo, int CreatedBy)
        {
            try
            {
                var customerCompanyDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == CreatedBy).FirstOrDefault();

                using (MailMessage mail = new MailMessage())
                {
                    string host = string.Empty;
                    int port = 0;
                    string userName = string.Empty;
                    string password = string.Empty;
                    string enableSsl = string.Empty;
                    string bccEmail = string.Empty;

                    if (customerCompanyDetail != null)
                    {
                        //HOST
                        host = customerCompanyDetail.SMTPHostName;

                        //PORT
                        port = Convert.ToInt32(customerCompanyDetail.SMTPport);

                        //USER NAME
                        userName = customerCompanyDetail.SMTPUserName;

                        //PASSWORD
                        password = customerCompanyDetail.SMTPPassword;

                        //Enable Ssl
                        enableSsl = customerCompanyDetail.SMTPEnableSsl;

                        //FROM MAIL
                        string fromMail = customerCompanyDetail.SMTPFromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, customerCompanyDetail.SMTPDisplayName);
                        mail.From = MailFrom;
                    }
                    else
                    {
                        //HOST
                        host = AppSettings.MailHostName;

                        //PORT
                        port = Convert.ToInt32(AppSettings.SMTPport);

                        //USER NAME
                        userName = AppSettings.SMTPUserName;

                        //PASSWORD
                        password = AppSettings.SMTPPassword;

                        //Enable Ssl
                        enableSsl = AppSettings.EnableSsl;

                        //BCC
                        bccEmail = AppSettings.BCC;


                        //FROM MAIL
                        string fromMail = AppSettings.FromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, AppSettings.DisplayName);
                        mail.From = MailFrom;
                    }

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (!string.IsNullOrEmpty(headerLogo))
                    {
                        Attachment logoImageAtt = new Attachment(headerLogo);
                        mail.Attachments.Add(logoImageAtt);
                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "FrayteLogo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail_ForgetPassword(string toEmail, string ccEmail, string subject, string mailContent, string Attachmentfilepath, string headerLogo, int id)
        {
            try
            {
                var customerCompanyDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == id).FirstOrDefault();

                using (MailMessage mail = new MailMessage())
                {
                    string host = string.Empty;
                    int port = 0;
                    string userName = string.Empty;
                    string password = string.Empty;
                    string enableSsl = string.Empty;

                    if (customerCompanyDetail != null)
                    {
                        //HOST
                        host = customerCompanyDetail.SMTPHostName;

                        //PORT
                        port = Convert.ToInt32(customerCompanyDetail.SMTPport);

                        //USER NAME
                        userName = customerCompanyDetail.SMTPUserName;

                        //PASSWORD
                        password = customerCompanyDetail.SMTPPassword;

                        //Enable Ssl
                        enableSsl = customerCompanyDetail.SMTPEnableSsl;

                        //FROM MAIL
                        string fromMail = customerCompanyDetail.SMTPFromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, customerCompanyDetail.SMTPDisplayName);
                        mail.From = MailFrom;
                    }
                    else
                    {
                        //HOST
                        host = AppSettings.MailHostName;

                        //PORT
                        port = Convert.ToInt32(AppSettings.SMTPport);

                        //USER NAME
                        userName = AppSettings.SMTPUserName;

                        //PASSWORD
                        password = AppSettings.SMTPPassword;

                        //Enable Ssl
                        enableSsl = AppSettings.EnableSsl;

                        //FROM MAIL
                        string fromMail = AppSettings.FromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, AppSettings.DisplayName);
                        mail.From = MailFrom;
                    }

                    //BCC
                    string bccEmail = AppSettings.BCC;
                     
                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                             
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (!string.IsNullOrEmpty(headerLogo))
                    {
                        System.Net.Mail.Attachment logoImageAtt = new System.Net.Mail.Attachment(headerLogo);
                        logoImageAtt.Name = System.IO.Path.GetFileName(headerLogo);
                        mail.Attachments.Add(logoImageAtt);

                        //Attachment logoImageAtt = new Attachment(headerLogo);
                        //mail.Attachments.Add(logoImageAtt);

                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "FrayteLogo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail(string toEmail, string ccEmail, string subject, string mailContent, string Attachmentfilepath, string headerLogo)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //HOST
                    string host = AppSettings.MailHostName;

                    //PORT
                    int port = Convert.ToInt32(AppSettings.SMTPport);

                    //USER NAME
                    string userName = AppSettings.SMTPUserName;

                    //PASSWORD
                    string password = AppSettings.SMTPPassword;

                    //Enable Ssl
                    string enableSsl = AppSettings.EnableSsl;

                    //BCC
                    string bccEmail = AppSettings.BCC;

                    //FROM MAIL
                    string fromMail = AppSettings.FromMail;
                    MailAddress MailFrom = new MailAddress(fromMail, AppSettings.DisplayName);
                    mail.From = MailFrom;

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (!string.IsNullOrEmpty(headerLogo))
                    {
                        Attachment logoImageAtt = new Attachment(headerLogo);
                        mail.Attachments.Add(logoImageAtt);
                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "FrayteLogo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail(string toEmail, string ccEmail, string subject, string mailContent, string headerLogo)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //HOST
                    string host = AppSettings.MailHostName;

                    //PORT
                    int port = Convert.ToInt32(AppSettings.SMTPport);

                    //USER NAME
                    string userName = AppSettings.SMTPUserName;

                    //PASSWORD
                    string password = AppSettings.SMTPPassword;

                    //Enable Ssl
                    string enableSsl = AppSettings.EnableSsl;

                    //BCC
                    string bccEmail = AppSettings.BCC;


                    //FROM MAIL
                    string fromMail = AppSettings.FromMail;
                    MailAddress MailFrom = new MailAddress(fromMail, AppSettings.DisplayName);
                    mail.From = MailFrom;

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    //string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    //for (int i = 0; i < strAttachmentPath.Length; i++)
                    //{
                    //    if (strAttachmentPath[i].Trim() != "")
                    //    {
                    //        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                    //        mail.Attachments.Add(attachment);
                    //    }
                    //}

                    if (!string.IsNullOrEmpty(headerLogo))
                    {
                        Attachment logoImageAtt = new Attachment(headerLogo);
                        mail.Attachments.Add(logoImageAtt);
                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "FrayteLogo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail(string toEmail, string ccEmail, string BccEmail, string subject, string mailContent, string Attachmentfilepath, string headerLogo)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //HOST
                    string host = AppSettings.MailHostName;

                    //PORT
                    int port = Convert.ToInt32(AppSettings.SMTPport);

                    //USER NAME
                    string userName = AppSettings.SMTPUserName;

                    //PASSWORD
                    string password = AppSettings.SMTPPassword;

                    //Enable Ssl
                    string enableSsl = AppSettings.EnableSsl;

                    //BCC
                    string bccEmail = BccEmail;


                    //FROM MAIL
                    string fromMail = AppSettings.FromMail;
                    MailAddress MailFrom = new MailAddress(fromMail, AppSettings.DisplayName);
                    mail.From = MailFrom;

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            attachment.Name = System.IO.Path.GetFileName(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (!string.IsNullOrEmpty(headerLogo))
                    {
                        Attachment logoImageAtt = new Attachment(headerLogo);
                        mail.Attachments.Add(logoImageAtt);
                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "FrayteLogo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail(string toEmail, string ccEmail, string BccEmail, string ReplyTo, string DisplayName, string subject, string mailContent, string Attachmentfilepath, string headerLogo, int CreatedBy)
        {
            try
            {
                var customerCompanyDetail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == CreatedBy).FirstOrDefault();

                using (MailMessage mail = new MailMessage())
                {
                    string host = string.Empty;
                    int port = 0;
                    string userName = string.Empty;
                    string password = string.Empty;
                    string enableSsl = string.Empty;
                    string bccEmail = string.Empty;

                    if (customerCompanyDetail != null)
                    {
                        //HOST
                        host = customerCompanyDetail.SMTPHostName;

                        //PORT
                        port = Convert.ToInt32(customerCompanyDetail.SMTPport);

                        //USER NAME
                        userName = customerCompanyDetail.SMTPUserName;

                        //PASSWORD
                        password = customerCompanyDetail.SMTPPassword;

                        //Enable Ssl
                        enableSsl = customerCompanyDetail.SMTPEnableSsl;

                        //FROM MAIL
                        string fromMail = customerCompanyDetail.SMTPFromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, DisplayName);
                        mail.From = MailFrom;
                    }
                    else
                    {
                        //HOST
                        host = AppSettings.MailHostName;

                        //PORT
                        port = Convert.ToInt32(AppSettings.SMTPport);

                        //USER NAME
                        userName = AppSettings.SMTPUserName;

                        //PASSWORD
                        password = AppSettings.SMTPPassword;

                        //Enable Ssl
                        enableSsl = AppSettings.EnableSsl;

                        //BCC
                        bccEmail = BccEmail;//AppSettings.BCC;

                        //FROM MAIL
                        string fromMail = AppSettings.FromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, DisplayName);
                        mail.From = MailFrom;
                    }

                    //REPLY TO
                    if (!string.IsNullOrEmpty(ReplyTo))
                    {
                        MailAddress MailReply = new MailAddress(ReplyTo);
                        mail.ReplyToList.Add(MailReply);
                    }

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    string strBCCEMail = string.Empty;
                    string[] BCCarray = bccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiBCC = BCCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiBCC.Length; i++)
                    {
                        strBCCEMail += Convert.ToString(DistiBCC[i]) + ";";
                    }

                    //BCC
                    string[] strBCCAdress = strBCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strBCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strBCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strBCCAdress[i].Trim());
                            mail.Bcc.Add(MailCC);
                        }
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (!string.IsNullOrEmpty(headerLogo))
                    {
                        Attachment logoImageAtt = new Attachment(headerLogo);
                        mail.Attachments.Add(logoImageAtt);
                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "FrayteLogo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail(string toEmail, string ccEmail, string BccEmail, string ReplyTo, string DisplayName, string subject, string mailContent, string Attachmentfilepath, string headerLogo)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //HOST
                    string host = AppSettings.MailHostName;

                    //PORT
                    int port = Convert.ToInt32(AppSettings.SMTPport);

                    //USER NAME
                    string userName = AppSettings.SMTPUserName;

                    //PASSWORD
                    string password = AppSettings.SMTPPassword;

                    //Enable Ssl
                    string enableSsl = AppSettings.EnableSsl;

                    //BCC
                    string bccEmail = BccEmail;//AppSettings.BCC;


                    //FROM MAIL
                    string fromMail = AppSettings.FromMail;
                    MailAddress MailFrom = new MailAddress(fromMail, DisplayName);
                    mail.From = MailFrom;

                    //REPLY TO
                    if (!string.IsNullOrEmpty(ReplyTo))
                    {
                        MailAddress MailReply = new MailAddress(ReplyTo);
                        mail.ReplyToList.Add(MailReply);
                    }

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    string strBCCEMail = string.Empty;
                    string[] BCCarray = bccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiBCC = BCCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiBCC.Length; i++)
                    {
                        strBCCEMail += Convert.ToString(DistiBCC[i]) + ";";
                    }

                    //BCC
                    string[] strBCCAdress = strBCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strBCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strBCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strBCCAdress[i].Trim());
                            mail.Bcc.Add(MailCC);
                        }
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (!string.IsNullOrEmpty(headerLogo))
                    {
                        Attachment logoImageAtt = new Attachment(headerLogo);
                        mail.Attachments.Add(logoImageAtt);
                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "FrayteLogo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail(string toEmail, string ccEmail, string DisplayName, string subject, string mailContent, string Attachmentfilepath, List<string> inlineImages, string Status)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //HOST
                    string host = AppSettings.MailHostName;

                    //PORT
                    int port = Convert.ToInt32(AppSettings.SMTPport);

                    //USER NAME
                    string userName = AppSettings.SMTPUserName;

                    //PASSWORD
                    string password = AppSettings.SMTPPassword;

                    //Enable Ssl
                    string enableSsl = AppSettings.EnableSsl;

                    //BCC
                    string bccEmail = AppSettings.BCC;


                    //FROM MAIL
                    string fromMail = AppSettings.FromMail;
                    MailAddress MailFrom = new MailAddress(fromMail, AppSettings.DisplayName);
                    mail.From = MailFrom;

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //To Dos
                    //    mail.ReplyTo = 

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (inlineImages.Count > 0)
                    {
                        foreach (string imagePath in inlineImages)
                        {
                            if (Status == "Confirmation")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("TrackShipment.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "TrackShipment";
                                }
                            }
                            else if (Status == "Cancel")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("Confirm.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Confirm";
                                }
                                else if (imagePath.Contains("Reject.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Reject";
                                }
                            }
                            else if (Status == "FuelCharge")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("FuelSurCharge.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FuelSurCharge";
                                }
                            }
                            else if (Status == "ExchangeRate")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("ExchangeRate.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "ExchangeRate";
                                }
                            }
                            else if (Status == "Amend")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("Confirm.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Confirm";
                                }
                                else if (imagePath.Contains("Reject.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Reject";
                                }
                                if (imagePath.Contains("Amend.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Amend";
                                }
                            }
                        }
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail(string toEmail, string ccEmail, string subject, string mailContent, string Attachmentfilepath, List<string> inlineImages, string Status, int OperationZoneId)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    int port = 0;
                    string host = string.Empty;
                    string userName = string.Empty;
                    string password = string.Empty;
                    string enableSsl = string.Empty;
                    string bccEmail = string.Empty;
                    string fromMail = string.Empty;

                    if (OperationZoneId == 1)
                    {
                        //HOST
                        host = AppSettings.HKHostName;

                        //PORT
                        port = Convert.ToInt32(AppSettings.HKport);

                        //USER NAME
                        userName = AppSettings.HKName;

                        //PASSWORD
                        password = AppSettings.HKPassword;

                        //Enable Ssl
                        enableSsl = AppSettings.HKEnableSsl;

                        //BCC
                        bccEmail = AppSettings.BCC;

                        //FROM MAIL
                        fromMail = AppSettings.HKFromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, AppSettings.HKDisplayName);
                        mail.From = MailFrom;
                    }
                    else if (OperationZoneId == 2)
                    {
                        //HOST
                        host = AppSettings.UKHostName;

                        //PORT
                        port = Convert.ToInt32(AppSettings.UKport);

                        //USER NAME
                        userName = AppSettings.UKName;

                        //PASSWORD
                        password = AppSettings.UKPassword;

                        //Enable Ssl
                        enableSsl = AppSettings.UKEnableSsl;

                        //BCC
                        bccEmail = AppSettings.BCC;

                        //FROM MAIL
                        fromMail = AppSettings.UKFromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, AppSettings.UKDisplayName);
                        mail.From = MailFrom;
                    }

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (inlineImages.Count > 0)
                    {
                        foreach (string imagePath in inlineImages)
                        {
                            if (Status == "Confirmation")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("TrackShipment.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "TrackShipment";
                                }
                            }
                            else if (Status == "Cancel")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("Confirm.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Confirm";
                                }
                                else if (imagePath.Contains("Reject.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Reject";
                                }
                            }
                            else if (Status == "FuelCharge")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("FuelSurCharge.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FuelSurCharge";
                                }
                            }
                            else if (Status == "ExchangeRate")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("ExchangeRate.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "ExchangeRate";
                                }
                            }
                            else if (Status == "Amend")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("Confirm.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Confirm";
                                }
                                else if (imagePath.Contains("Reject.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Reject";
                                }
                                if (imagePath.Contains("Amend.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Amend";
                                }
                            }
                        }
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendMail(string toEmail, string ccEmail, string subject, string mailContent, List<string> Attachmentfilepath, string headerLogo, int OperationZoneId)
        {
            string filePath = @"C:\FMS\FRAYTE HK\Email.txt";
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    int port = 0;
                    string host = string.Empty;
                    string userName = string.Empty;
                    string password = string.Empty;
                    string enableSsl = string.Empty;
                    string bccEmail = string.Empty;
                    string fromMail = string.Empty;

                    if (OperationZoneId == 1)
                    {
                        //HOST
                        host = AppSettings.HKHostName;

                        //PORT
                        port = Convert.ToInt32(AppSettings.HKport);

                        //USER NAME
                        userName = AppSettings.HKName;

                        //PASSWORD
                        password = AppSettings.HKPassword;

                        //Enable Ssl
                        enableSsl = AppSettings.HKEnableSsl;

                        //BCC
                        bccEmail = AppSettings.BCC;

                        //FROM MAIL
                        fromMail = AppSettings.HKFromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, AppSettings.HKDisplayName);
                        mail.From = MailFrom;

                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true))
                        {
                            writer.WriteLine(host + Environment.NewLine + port + Environment.NewLine + userName + Environment.NewLine + password +
                               "" + Environment.NewLine + enableSsl + Environment.NewLine + bccEmail + Environment.NewLine + fromMail + Environment.NewLine + AppSettings.HKDisplayName);
                            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                        }
                    }
                    else if (OperationZoneId == 2)
                    {
                        //HOST
                        host = AppSettings.UKHostName;

                        //PORT
                        port = Convert.ToInt32(AppSettings.UKport);

                        //USER NAME
                        userName = AppSettings.UKName;

                        //PASSWORD
                        password = AppSettings.UKPassword;

                        //Enable Ssl
                        enableSsl = AppSettings.UKEnableSsl;

                        //BCC
                        bccEmail = AppSettings.BCC;

                        //FROM MAIL
                        fromMail = AppSettings.UKFromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, AppSettings.UKDisplayName);
                        mail.From = MailFrom;

                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true))
                        {
                            writer.WriteLine(host + Environment.NewLine + port + Environment.NewLine + userName + Environment.NewLine + password +
                               "" + Environment.NewLine + enableSsl + Environment.NewLine + bccEmail + Environment.NewLine + fromMail + Environment.NewLine + AppSettings.UKDisplayName);
                            writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                        }
                    }

                    //Take Distinct To Mail ID's
                    string strTOEMail = string.Empty;
                    string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiTO = TOarray.Distinct().ToArray();
                    for (int i = 0; i < DistiTO.Length; i++)
                    {
                        strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                    }

                    //TO
                    string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strToAdress.Length; i++)
                    {
                        if (Convert.ToString(strToAdress[i]).Trim() != "")
                        {
                            MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                            mail.To.Add(MailTo);
                        }
                    }

                    //Take Distinct CC Mail ID's
                    string strCCEMail = string.Empty;
                    string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] DistiCC = CCarray.Distinct().ToArray();
                    for (int i = 0; i < DistiCC.Length; i++)
                    {
                        strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                    }

                    //CC
                    string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strCCAdress.Length; i++)
                    {
                        if (Convert.ToString(strCCAdress[i]).Trim() != "")
                        {
                            MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                            mail.CC.Add(MailCC);
                        }
                    }

                    //BCC
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        mail.Bcc.Add(bccEmail);
                    }

                    //To Dos
                    //mail.ReplyTo

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    if (Attachmentfilepath.Count > 0)
                    {
                        for (int i = 0; i < Attachmentfilepath.Count; i++)
                        {
                            if (Attachmentfilepath[i].Trim() != "")
                            {
                                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(Attachmentfilepath[i].Trim());
                                attachment.Name = System.IO.Path.GetFileName(Attachmentfilepath[i].Trim());
                                mail.Attachments.Add(attachment);

                                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true))
                                {
                                    writer.WriteLine(attachment.Name + Environment.NewLine + Attachmentfilepath[i] + Environment.NewLine);
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(headerLogo))
                    {
                        Attachment logoImageAtt = new Attachment(headerLogo);
                        mail.Attachments.Add(logoImageAtt);
                        logoImageAtt.ContentDisposition.Inline = true;
                        logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        logoImageAtt.ContentId = "FrayteLogo";
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true))
                {
                    writer.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.Source + Environment.NewLine + ex.InnerException +
                       "" + Environment.NewLine + ex.Source);
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
                throw ex;
            }
        }

        #region PreAlert Email

        public static void SendPreAlertMail(string toEmail, string ccEmail, string bccEmails, string replyTo, string subject, string mailContent, string Attachmentfilepath, List<string> inlineImages, string Status)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    //HOST
                    string host = AppSettings.MailHostName;

                    //PORT
                    int port = Convert.ToInt32(AppSettings.SMTPport);

                    //USER NAME
                    string userName = AppSettings.SMTPUserName;

                    //PASSWORD
                    string password = AppSettings.SMTPPassword;

                    //Enable Ssl
                    string enableSsl = AppSettings.EnableSsl;

                    //BCC
                    string bccEmail = bccEmails;

                    string TOCC = AppSettings.TOCC;

                    if (!string.IsNullOrEmpty(TOCC))
                    {
                        string fromMail = AppSettings.FromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, AppSettings.DisplayName);
                        mail.From = MailFrom;

                        MailAddress MailTo = new MailAddress(TOCC);
                        mail.To.Add(MailTo);
                        
                    }
                    else
                    {
                        //FROM MAIL
                        string fromMail = AppSettings.FromMail;
                        MailAddress MailFrom = new MailAddress(fromMail, AppSettings.DisplayName);
                        mail.From = MailFrom;

                        //Take Distinct To Mail ID's
                        string strTOEMail = string.Empty;
                        string[] TOarray = toEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                        string[] DistiTO = TOarray.Distinct().ToArray();
                        for (int i = 0; i < DistiTO.Length; i++)
                        {
                            strTOEMail += Convert.ToString(DistiTO[i]) + ";";
                        }

                        //TO
                        string[] strToAdress = strTOEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                        for (int i = 0; i < strToAdress.Length; i++)
                        {
                            if (Convert.ToString(strToAdress[i]).Trim() != "")
                            {
                                MailAddress MailTo = new MailAddress(strToAdress[i].Trim());
                                mail.To.Add(MailTo);
                            }
                        }

                        //Take Distinct CC Mail ID's
                        string strCCEMail = string.Empty;
                        string[] CCarray = ccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                        string[] DistiCC = CCarray.Distinct().ToArray();
                        for (int i = 0; i < DistiCC.Length; i++)
                        {
                            strCCEMail += Convert.ToString(DistiCC[i]) + ";";
                        }

                        //CC
                        string[] strCCAdress = strCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                        for (int i = 0; i < strCCAdress.Length; i++)
                        {
                            if (Convert.ToString(strCCAdress[i]).Trim() != "")
                            {
                                MailAddress MailCC = new MailAddress(strCCAdress[i].Trim());
                                mail.CC.Add(MailCC);
                            }
                        }

                        //Take Distinct BCC Mail ID's
                        string strBCCEMail = string.Empty;
                        string[] BCCarray = bccEmail.Split(new string[] { ";" }, StringSplitOptions.None);
                        string[] DistiBCC = BCCarray.Distinct().ToArray();
                        for (int i = 0; i < DistiBCC.Length; i++)
                        {
                            strBCCEMail += Convert.ToString(DistiBCC[i]) + ";";
                        }
                        //BCC
                        string[] strBCCAdress = strBCCEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                        for (int i = 0; i < strBCCAdress.Length; i++)
                        {
                            if (Convert.ToString(strBCCAdress[i]).Trim() != "")
                            {
                                MailAddress MailBCC = new MailAddress(strBCCAdress[i].Trim());
                                mail.Bcc.Add(MailBCC);
                            }
                        }



                        //Take Distinct replyTo Mail ID's
                        string strRPTEMail = string.Empty;
                        string[] RPTarray = replyTo.Split(new string[] { ";" }, StringSplitOptions.None);
                        string[] DistiRPT = RPTarray.Distinct().ToArray();
                        for (int i = 0; i < DistiRPT.Length; i++)
                        {
                            strRPTEMail += Convert.ToString(DistiRPT[i]) + ";";
                        }
                        //Reply To
                        // mail.ReplyToList =
                        string[] strRPTAdress = strRPTEMail.Split(new string[] { ";" }, StringSplitOptions.None);
                        for (int i = 0; i < strRPTAdress.Length; i++)
                        {
                            if (Convert.ToString(strRPTAdress[i]).Trim() != "")
                            {
                                MailAddress MailRPT = new MailAddress(strRPTAdress[i].Trim());
                                mail.ReplyToList.Add(MailRPT);
                            }
                        }
                    }

                    //SUBJECT
                    mail.Subject = subject;

                    //BODY
                    mail.Body = mailContent;
                    mail.IsBodyHtml = true;

                    string[] strAttachmentPath = Attachmentfilepath.Split(new string[] { ";" }, StringSplitOptions.None);
                    for (int i = 0; i < strAttachmentPath.Length; i++)
                    {
                        if (strAttachmentPath[i].Trim() != "")
                        {
                            //System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            //mail.Attachments.Add(attachment);
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(strAttachmentPath[i].Trim());
                            attachment.Name = System.IO.Path.GetFileName(strAttachmentPath[i].Trim());
                            mail.Attachments.Add(attachment);
                        }
                    }

                    if (inlineImages.Count > 0)
                    {
                        foreach (string imagePath in inlineImages)
                        {
                            if (Status == "Confirmation")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("TrackShipment.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "TrackShipment";
                                }
                            }
                            else if (Status == "Cancel")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("Confirm.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Confirm";
                                }
                                else if (imagePath.Contains("Reject.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Reject";
                                }
                            }
                            else if (Status == "FuelCharge")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("FuelSurCharge.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FuelSurCharge";
                                }
                            }
                            else if (Status == "ExchangeRate")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("ExchangeRate.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "ExchangeRate";
                                }
                            }
                            else if (Status == "Amend")
                            {
                                if (imagePath.Contains("FrayteLogo.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "FrayteLogo";
                                }
                                else if (imagePath.Contains("Confirm.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Confirm";
                                }
                                else if (imagePath.Contains("Reject.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Reject";
                                }
                                if (imagePath.Contains("Amend.png"))
                                {
                                    Attachment logoImageAtt = new Attachment(imagePath);
                                    mail.Attachments.Add(logoImageAtt);
                                    logoImageAtt.ContentDisposition.Inline = true;
                                    logoImageAtt.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                                    logoImageAtt.ContentId = "Amend";
                                }
                            }
                        }
                    }

                    //SMTP
                    SmtpClient smtp = new SmtpClient { Host = host, Port = port };
                    smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    smtp.EnableSsl = enableSsl == "N" ? false : true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}