using System;
using System.Collections.Generic;
using Serilog;
using System.Net.Sockets;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace ModniteServer.Xmpp
{
    internal sealed class XmppClient
    {
        public XmppClient(XmppServer owner, Socket socket)
        {
            Server = owner;
            Socket = socket;
        }

        public XmppServer Server { get; }

        public Socket Socket { get; }

        private string AccountID { get; set; }

        private string Resource { get; set; }

        internal void HandleMessage(XElement element, out XElement response)
        {
            bool messageHandled = false;

            switch (element.Name.LocalName)
            {
                case "iq":
                    {
                        string id = element.Attribute("id").Value;
                        string type = element.Attribute("type").Value;

                        if (id == "_xmpp_auth1")
                        {
                            XNamespace authNs = "jabber:iq:auth";
                            var query = element.Element(authNs + "query");

                            string username = query.Element(authNs + "username").Value;
                            string password = query.Element(authNs + "password").Value;
                            string resource = query.Element(authNs + "resource").Value;

                            Log.Information("[XMPP] Login requested for '" + username + "'");
                            AccountID = username;
                            Resource = resource;

                            var loginSuccessfulResponse = new XElement("iq");
                            loginSuccessfulResponse.Add(new XAttribute("type", "result"), new XAttribute("id", "_xmpp_auth1"));

                            Server.SendXmppMessage(Socket, loginSuccessfulResponse);
                            messageHandled = true;
                        }
                        else
                        {
                            // Handle ping.
                            XNamespace pingNs = "urn:xmpp:ping";
                            var pingElement = element.Element(pingNs + "ping");
                            if (pingElement != null)
                            {
                                string from = element.Attribute("from").Value;
                                string to = element.Attribute("to").Value;

                                var pongResponse = new XElement(
                                    "iq",
                                    new XAttribute("id", id),
                                    new XAttribute("from", from),
                                    new XAttribute("to", to),
                                    new XAttribute("type", "result")
                                );

                                Log.Information("[XMPP] Ping");

                                Server.SendXmppMessage(Socket, pongResponse);
                                messageHandled = true;
                            }

                            // TODO: Handle other <iq/> messages here.
                        }
                    }
                    break;

                case "close":
                    {
                        var closeResponse = new XElement("close", new XAttribute("xmlns", "urn:ietf:params:xml:ns:xmpp-framing"));
                        Server.SendXmppMessage(Socket, closeResponse);
                        messageHandled = true;
                    }
                    break;

                case "presence":
                    {
                        var rResponse = new XElement("presence",
                            new XAttribute("to", $"{AccountID}@prod.ol.epicgames.com/{Resource}"),
                            new XAttribute("from", $"{AccountID}@prod.ol.epicgames.com/{Resource}"),
                            new XElement("status", JsonConvert.SerializeObject(new
                            {
                                Status = "",
                                bIsPlaying = false,
                                bHasVoiceSupport = false,
                                SessionId = "",
                                Properties = new { }
                            })),
                            new XElement("priority", 0),
                            new XElement("delay",
                                new XAttribute("xlmns", "urn:xmpp:delay"),
                                new XAttribute("stamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))));

                        Server.SendXmppMessage(Socket, rResponse);

                        rResponse = new XElement("presence",
                            new XAttribute("forwarded-packet", true),
                            new XAttribute("to", $"{AccountID}@prod.ol.epicgames.com/{Resource}"),
                            new XAttribute("CLUSTER_HOPS_AMOUNT", 1),
                            new XAttribute("from", $"{AccountID}@prod.ol.epicgames.com/{Resource}"),
                            new XElement("show", "xa"),
                            new XElement("status", JsonConvert.SerializeObject(new
                            {
                                Status = "Battle Royale Lobby - 1 / 4", // change when you can actually join people (if ever)
                                bIsPlaying = false,
                                bIsJoinable = false,
                                bHasVoiceSupport = false,
                                SessionId = "",
                                Properties = new
                                {
                                    FortBasicInfo_j = new
                                    {
                                        homeBaseRating = 1
                                    },
                                    FortLFG_I = "0",
                                    FortPartySize_i = 1,
                                    FortSubGame_i = 1,
                                    InUnjoinableMatch_b = false,
                                    Event_Level_s = "0",
                                    Event_Rating_u = 1,
                                    RichPresence_s = "AthenaLobby",
                                    Event_PartySize_s = "1",
                                    Event_PartyMaxSize_s = "4",
                                    Event_PlayersAlive_s = "48"
                                }
                            })),
                            new XElement("delay",
                                new XAttribute("xlmns", "urn:xmpp:delay"),
                                new XAttribute("stamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))));

                        Server.SendXmppMessage(Socket, rResponse);

                        rResponse = new XElement("presence",
                            new XAttribute("forwarded-packet", true),
                            new XAttribute("to", $"{AccountID}@prod.ol.epicgames.com/{Resource}"),
                            new XAttribute("CLUSTER_HOPS_AMOUNT", 1),
                            new XAttribute("from", $"{AccountID}@prod.ol.epicgames.com/{Resource}"),
                            new XElement("status", JsonConvert.SerializeObject(new
                            {
                                Status = "",
                                bIsPlaying = false,
                                bIsJoinable = false,
                                bHasVoiceSupport = false,
                                SessionId = "",
                                Properties = new { }
                            })),
                            new XElement("priority", 0),
                            new XElement("delay",
                                new XAttribute("xlmns", "urn:xmpp:delay"),
                                new XAttribute("stamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))));

                        Server.SendXmppMessage(Socket, rResponse);
                        messageHandled = true;
                    }
                    break;
            }

            if (!messageHandled)
            {
                Log.Warning("[XMPP] Unhandled message received {Message}", element);
            }

            response = null;
        }
    }
}
