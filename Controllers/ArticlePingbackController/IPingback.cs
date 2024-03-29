﻿/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using CookComputing.XmlRpc;
namespace ArticlePingbackController
{
    [XmlRpcUrl("http://gokk.com")]
    public interface IPingback : IXmlRpcProxy
    {
        [XmlRpcMethod("pingback.ping")]
        void Pingback(string source, string destination);
    }
}
