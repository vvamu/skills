﻿using EmailProvider.Models;
using System.Threading.Tasks;

namespace EmailProvider.Interfaces;

public interface IMailService
{
    public Task SendEmailAsync(SendingMessage mailrequest);
    public Task SendEmailAsync(Mailrequest mailrequest);

}
