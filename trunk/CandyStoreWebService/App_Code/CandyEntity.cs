/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;

public class CandyEntity
{
    private string _candyName;
    private string _candyImageUrl;

    public string CandyName
    {
        get { return _candyName; }
        set { _candyName = value; }
    }

    public string CandyImageUrl
    {
        get { return _candyImageUrl; }
        set { _candyImageUrl = value; }
    }

    public CandyEntity()
    {
    }
}
