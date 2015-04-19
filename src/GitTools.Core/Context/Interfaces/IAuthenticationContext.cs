// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationContext.cs" company="GitTools">
//   Copyright (c) 2014 - 2015 GitTools. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitTools
{
    public interface IAuthenticationContext
    {
        string Username { get; set; }

        string Password { get; set; }
    }
}