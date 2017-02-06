﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WHMCS_API
{
    public class API
    {
        private readonly Call _call;
        public API(string Username, string Password, string AccessKey, string Url)
        {
            _call = new Call(Username, Password, AccessKey, Url);
        }

        /// <summary>
        /// Validates a client login
        /// </summary>
        /// <see cref="https://developers.whmcs.com/api-reference/validatelogin/"/>
        /// <param name="Email">Client Email</param>
        /// <param name="Password">Client Password (cleartext)</param>
        /// <returns>Returns the result of the call
        /// The userid string for the session Session["uid"]
        /// The passwordhash for the session Session["upw"]</returns>
        public ValidateLogin ValidateLogin(string Email, string Password)
        {
            NameValueCollection data = new NameValueCollection()
            {
                { "action", APIEnums.Actions.ValidateLogin.ToString() },
                { EnumUtil.GetString(APIEnums.ValidateLoginParams.Email), Email },
                { EnumUtil.GetString(APIEnums.ValidateLoginParams.Password), Password }
            };

            return JsonConvert.DeserializeObject<ValidateLogin>(_call.MakeCall(data));
        }

        /// <summary>
        /// Checks if a domain is available to regsiter, if not will return the whois
        /// </summary>
        /// <see cref="https://developers.whmcs.com/api-reference/domainwhois/"/>
        /// <param name="Domain">The domain to be checked</param>
        /// <returns>Result of the call, if the domain is registered or not, and if registered the WhoIs</returns>
        public DomainWhoIs DomainWhoIs(string Domain)
        {
            NameValueCollection data = new NameValueCollection()
            {
                { "action", APIEnums.Actions.DomainWhois.ToString() },
                { EnumUtil.GetString(APIEnums.DomainWhoisParams.Domain), Domain },
            };

            return JsonConvert.DeserializeObject<DomainWhoIs>(_call.MakeCall(data));
        }

        /// <summary>
        /// Registers a new client
        /// </summary>
        /// <remarks>
        /// When registerying an exception may occurr. If you get "An API Error Ocurred" read the inner exception
        /// </remarks>
        /// <example>
        /// try { your code } catch (Exception ex) { Console.WriteLine(ex.InnerException.Message); }
        /// </example>
        /// <see cref="https://developers.whmcs.com/api-reference/addclient/"/>
        /// <param name="ClientInfo">The Model of the AddClient action</param>
        /// <returns>If success returns the ID of the newly created user otherwise will throw an exception</returns>
        public int AddClient(AddClient ClientInfo)
        {
            NameValueCollection data = new NameValueCollection()
            {
                { "action", APIEnums.Actions.AddClient.ToString() }
            };

            //Processes all the data in ClientInfo model into the data NameValueCollection
            foreach (string key in ClientInfo.ClientInfo)
            {
                data.Add(key, ClientInfo.ClientInfo[key]);
            }

            JObject result = JObject.Parse(_call.MakeCall(data));

            if (result["result"].ToString() == "success")
                return Convert.ToInt32(result["clientid"]);
            else
                throw new Exception("An API Error Ocurred", new Exception(result["message"].ToString()));

        }

        /// <summary>
        /// Gets the client details
        /// </summary>
        /// <param name="ClientID">The client ID to search</param>
        /// <param name="ClientEmail">The client Email to search</param>
        /// <param name="Stats">Get extended stats for the client</param>
        /// <returns>All details of the client</returns>
        public GetClientsDetails GetClientsDetails(int ClientID = -1, string ClientEmail = "", bool Stats = false)
        {
            if (ClientID == -1 && ClientEmail == "")
                throw new Exception("ClientID or ClientEmail needed");

            NameValueCollection data = new NameValueCollection()
            {
                { "action", APIEnums.Actions.GetClientsDetails.ToString() },
                { EnumUtil.GetString(APIEnums.GetClientsDetailsParams.Stats), Stats.ToString() },
            };
            if (ClientID != -1)
                data.Add(EnumUtil.GetString(APIEnums.GetClientsDetailsParams.ClientID), ClientID.ToString());
            if (ClientEmail != "" && ClientID == -1)
                data.Add(EnumUtil.GetString(APIEnums.GetClientsDetailsParams.Email), ClientEmail);

            string req = _call.MakeCall(data);
            JObject result = JObject.Parse(req);
            if (result["result"].ToString() == "success")
                return JsonConvert.DeserializeObject<GetClientsDetails>(req);
            else
                throw new Exception("An API Error occurred", new Exception(result["message"].ToString()));
        }
    }
}