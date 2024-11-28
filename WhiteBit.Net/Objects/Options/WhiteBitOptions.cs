using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteBit.Net.Objects.Options
{
    /// <summary>
    /// WhiteBit options
    /// </summary>
    public class WhiteBitOptions : LibraryOptions<WhiteBitRestOptions, WhiteBitSocketOptions, ApiCredentials, WhiteBitEnvironment>
    {
    }
}
