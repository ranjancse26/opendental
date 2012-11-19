using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Drawing;

namespace OpenDentalWebService {
	///<summary>This file is generated automatically by the crud, do not make any changes to this file because they will get overwritten.</summary>
	public class EobAttach {

		///<summary></summary>
		public static string Serialize(OpenDentBusiness.EobAttach eobattach) {
			StringBuilder sb=new StringBuilder();
			sb.Append("<EobAttach>");
			sb.Append("<EobAttachNum>").Append(eobattach.EobAttachNum).Append("</EobAttachNum>");
			sb.Append("<ClaimPaymentNum>").Append(eobattach.ClaimPaymentNum).Append("</ClaimPaymentNum>");
			sb.Append("<DateTCreated>").Append(eobattach.DateTCreated.ToString("yyyyMMddHHmmss")).Append("</DateTCreated>");
			sb.Append("<FileName>").Append(SerializeStringEscapes.EscapeForXml(eobattach.FileName)).Append("</FileName>");
			sb.Append("<RawBase64>").Append(SerializeStringEscapes.EscapeForXml(eobattach.RawBase64)).Append("</RawBase64>");
			sb.Append("</EobAttach>");
			return sb.ToString();
		}

		///<summary></summary>
		public static OpenDentBusiness.EobAttach Deserialize(string xml) {
			OpenDentBusiness.EobAttach eobattach=new OpenDentBusiness.EobAttach();
			using(XmlReader reader=XmlReader.Create(new StringReader(xml))) {
				reader.MoveToContent();
				while(reader.Read()) {
					//Only detect start elements.
					if(!reader.IsStartElement()) {
						continue;
					}
					switch(reader.Name) {
						case "EobAttachNum":
							eobattach.EobAttachNum=System.Convert.ToInt64(reader.ReadContentAsString());
							break;
						case "ClaimPaymentNum":
							eobattach.ClaimPaymentNum=System.Convert.ToInt64(reader.ReadContentAsString());
							break;
						case "DateTCreated":
							eobattach.DateTCreated=DateTime.ParseExact(reader.ReadContentAsString(),"yyyyMMddHHmmss",null);
							break;
						case "FileName":
							eobattach.FileName=reader.ReadContentAsString();
							break;
						case "RawBase64":
							eobattach.RawBase64=reader.ReadContentAsString();
							break;
					}
				}
			}
			return eobattach;
		}


	}
}