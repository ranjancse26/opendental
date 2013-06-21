//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class EmailMessageCrud {
		///<summary>Gets one EmailMessage object from the database using the primary key.  Returns null if not found.</summary>
		public static EmailMessage SelectOne(long emailMessageNum){
			string command="SELECT * FROM emailmessage "
				+"WHERE EmailMessageNum = "+POut.Long(emailMessageNum);
			List<EmailMessage> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one EmailMessage object from the database using a query.</summary>
		public static EmailMessage SelectOne(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EmailMessage> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of EmailMessage objects from the database using a query.</summary>
		public static List<EmailMessage> SelectMany(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EmailMessage> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<EmailMessage> TableToList(DataTable table){
			List<EmailMessage> retVal=new List<EmailMessage>();
			EmailMessage emailMessage;
			for(int i=0;i<table.Rows.Count;i++) {
				emailMessage=new EmailMessage();
				emailMessage.EmailMessageNum= PIn.Long  (table.Rows[i]["EmailMessageNum"].ToString());
				emailMessage.PatNum         = PIn.Long  (table.Rows[i]["PatNum"].ToString());
				emailMessage.ToAddress      = PIn.String(table.Rows[i]["ToAddress"].ToString());
				emailMessage.FromAddress    = PIn.String(table.Rows[i]["FromAddress"].ToString());
				emailMessage.Subject        = PIn.String(table.Rows[i]["Subject"].ToString());
				emailMessage.BodyText       = PIn.String(table.Rows[i]["BodyText"].ToString());
				emailMessage.MsgDateTime    = PIn.DateT (table.Rows[i]["MsgDateTime"].ToString());
				emailMessage.SentOrReceived = (EmailSentOrReceived)PIn.Int(table.Rows[i]["SentOrReceived"].ToString());
				retVal.Add(emailMessage);
			}
			return retVal;
		}

		///<summary>Inserts one EmailMessage into the database.  Returns the new priKey.</summary>
		public static long Insert(EmailMessage emailMessage){
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				emailMessage.EmailMessageNum=DbHelper.GetNextOracleKey("emailmessage","EmailMessageNum");
				int loopcount=0;
				while(loopcount<100){
					try {
						return Insert(emailMessage,true);
					}
					catch(Oracle.DataAccess.Client.OracleException ex){
						if(ex.Number==1 && ex.Message.ToLower().Contains("unique constraint") && ex.Message.ToLower().Contains("violated")){
							emailMessage.EmailMessageNum++;
							loopcount++;
						}
						else{
							throw ex;
						}
					}
				}
				throw new ApplicationException("Insert failed.  Could not generate primary key.");
			}
			else {
				return Insert(emailMessage,false);
			}
		}

		///<summary>Inserts one EmailMessage into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(EmailMessage emailMessage,bool useExistingPK){
			if(!useExistingPK && PrefC.RandomKeys) {
				emailMessage.EmailMessageNum=ReplicationServers.GetKey("emailmessage","EmailMessageNum");
			}
			string command="INSERT INTO emailmessage (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="EmailMessageNum,";
			}
			command+="PatNum,ToAddress,FromAddress,Subject,BodyText,MsgDateTime,SentOrReceived) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(emailMessage.EmailMessageNum)+",";
			}
			command+=
				     POut.Long  (emailMessage.PatNum)+","
				+"'"+POut.String(emailMessage.ToAddress)+"',"
				+"'"+POut.String(emailMessage.FromAddress)+"',"
				+"'"+POut.String(emailMessage.Subject)+"',"
				+"'"+POut.String(emailMessage.BodyText)+"',"
				+    POut.DateT (emailMessage.MsgDateTime)+","
				+    POut.Int   ((int)emailMessage.SentOrReceived)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				emailMessage.EmailMessageNum=Db.NonQ(command,true);
			}
			return emailMessage.EmailMessageNum;
		}

		///<summary>Updates one EmailMessage in the database.</summary>
		public static void Update(EmailMessage emailMessage){
			string command="UPDATE emailmessage SET "
				+"PatNum         =  "+POut.Long  (emailMessage.PatNum)+", "
				+"ToAddress      = '"+POut.String(emailMessage.ToAddress)+"', "
				+"FromAddress    = '"+POut.String(emailMessage.FromAddress)+"', "
				+"Subject        = '"+POut.String(emailMessage.Subject)+"', "
				+"BodyText       = '"+POut.String(emailMessage.BodyText)+"', "
				+"MsgDateTime    =  "+POut.DateT (emailMessage.MsgDateTime)+", "
				+"SentOrReceived =  "+POut.Int   ((int)emailMessage.SentOrReceived)+" "
				+"WHERE EmailMessageNum = "+POut.Long(emailMessage.EmailMessageNum);
			Db.NonQ(command);
		}

		///<summary>Updates one EmailMessage in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.</summary>
		public static void Update(EmailMessage emailMessage,EmailMessage oldEmailMessage){
			string command="";
			if(emailMessage.PatNum != oldEmailMessage.PatNum) {
				if(command!=""){ command+=",";}
				command+="PatNum = "+POut.Long(emailMessage.PatNum)+"";
			}
			if(emailMessage.ToAddress != oldEmailMessage.ToAddress) {
				if(command!=""){ command+=",";}
				command+="ToAddress = '"+POut.String(emailMessage.ToAddress)+"'";
			}
			if(emailMessage.FromAddress != oldEmailMessage.FromAddress) {
				if(command!=""){ command+=",";}
				command+="FromAddress = '"+POut.String(emailMessage.FromAddress)+"'";
			}
			if(emailMessage.Subject != oldEmailMessage.Subject) {
				if(command!=""){ command+=",";}
				command+="Subject = '"+POut.String(emailMessage.Subject)+"'";
			}
			if(emailMessage.BodyText != oldEmailMessage.BodyText) {
				if(command!=""){ command+=",";}
				command+="BodyText = '"+POut.String(emailMessage.BodyText)+"'";
			}
			if(emailMessage.MsgDateTime != oldEmailMessage.MsgDateTime) {
				if(command!=""){ command+=",";}
				command+="MsgDateTime = "+POut.DateT(emailMessage.MsgDateTime)+"";
			}
			if(emailMessage.SentOrReceived != oldEmailMessage.SentOrReceived) {
				if(command!=""){ command+=",";}
				command+="SentOrReceived = "+POut.Int   ((int)emailMessage.SentOrReceived)+"";
			}
			if(command==""){
				return;
			}
			command="UPDATE emailmessage SET "+command
				+" WHERE EmailMessageNum = "+POut.Long(emailMessage.EmailMessageNum);
			Db.NonQ(command);
		}

		///<summary>Deletes one EmailMessage from the database.</summary>
		public static void Delete(long emailMessageNum){
			string command="DELETE FROM emailmessage "
				+"WHERE EmailMessageNum = "+POut.Long(emailMessageNum);
			Db.NonQ(command);
		}

	}
}