package com.opendental.odweb.client.tabletypes;

import com.google.gwt.xml.client.Document;
import com.google.gwt.xml.client.XMLParser;
import com.opendental.odweb.client.remoting.Serializing;
import com.google.gwt.i18n.client.DateTimeFormat;
import java.util.Date;

public class SupplyNeeded {
		/** Primary key. */
		public int SupplyNeededNum;
		/** . */
		public String Description;
		/** . */
		public Date DateAdded;

		/** Deep copy of object. */
		public SupplyNeeded Copy() {
			SupplyNeeded supplyneeded=new SupplyNeeded();
			supplyneeded.SupplyNeededNum=this.SupplyNeededNum;
			supplyneeded.Description=this.Description;
			supplyneeded.DateAdded=this.DateAdded;
			return supplyneeded;
		}

		/** Serialize the object into XML. */
		public String SerializeToXml() {
			StringBuilder sb=new StringBuilder();
			sb.append("<SupplyNeeded>");
			sb.append("<SupplyNeededNum>").append(SupplyNeededNum).append("</SupplyNeededNum>");
			sb.append("<Description>").append(Serializing.EscapeForXml(Description)).append("</Description>");
			sb.append("<DateAdded>").append(DateTimeFormat.getFormat("yyyyMMddHHmmss").format(DateAdded)).append("</DateAdded>");
			sb.append("</SupplyNeeded>");
			return sb.toString();
		}

		/** Sets the variables for this object based on the values from the XML.
		 * @param xml The XML passed in must be valid and contain a node for every variable on this object.
		 * @throws Exception Deserialize is encased in a try catch and will pass any thrown exception on. */
		public void DeserializeFromXml(String xml) throws Exception {
			try {
				Document doc=XMLParser.parse(xml);
				if(Serializing.GetXmlNodeValue(doc,"SupplyNeededNum")!=null) {
					SupplyNeededNum=Integer.valueOf(Serializing.GetXmlNodeValue(doc,"SupplyNeededNum"));
				}
				if(Serializing.GetXmlNodeValue(doc,"Description")!=null) {
					Description=Serializing.GetXmlNodeValue(doc,"Description");
				}
				if(Serializing.GetXmlNodeValue(doc,"DateAdded")!=null) {
					DateAdded=DateTimeFormat.getFormat("yyyyMMddHHmmss").parseStrict(Serializing.GetXmlNodeValue(doc,"DateAdded"));
				}
			}
			catch(Exception e) {
				throw e;
			}
		}


}