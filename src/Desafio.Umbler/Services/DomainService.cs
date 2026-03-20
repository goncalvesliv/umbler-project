using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Desafio.Umbler.Models;
using Desafio.Umbler.ViewModels;
using DnsClient;
using Microsoft.EntityFrameworkCore;
using Whois.NET;

namespace Desafio.Umbler.Services
{
    public class DomainService : IDomainService
    {
        private readonly DatabaseContext _db;

        public DomainService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<DomainViewModel> GetDomainInfoAsync(string domainName)
        {
            var domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);

            bool needsUpdate = domain == null || 
                               DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl;

            if (needsUpdate)
            {
                var (ip, ttl, whoisRaw, hostedAt) = await FetchDomainDataAsync(domainName);

                if (domain == null)
                {
                    domain = new Domain { Name = domainName };
                    _db.Domains.Add(domain);
                }

                domain.Ip = ip;
                domain.UpdatedAt = DateTime.Now;
                domain.WhoIs = whoisRaw;
                domain.Ttl = ttl;
                domain.HostedAt = hostedAt;

                await _db.SaveChangesAsync();
            }

            return ToViewModel(domain);
        }

        private async Task<(string ip, int ttl, string whoisRaw, string hostedAt)> FetchDomainDataAsync(string domainName)
        {
            var response = await WhoisClient.QueryAsync(domainName);

            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domainName, QueryType.ANY);
            var record = result.Answers.ARecords().FirstOrDefault();
            var ip = record?.Address?.ToString();

            var hostResponse = await WhoisClient.QueryAsync(ip);

            return (ip, record?.TimeToLive ?? 0, response.Raw, hostResponse.OrganizationName);
        }

        private DomainViewModel ToViewModel(Domain domain)
        {
            var nameServers = domain.WhoIs?
                .Split('\n')
                .Where(l => l.TrimStart().StartsWith("Name Server:"))
                .Select(l => l.Replace("Name Server:", "").Trim())
                .ToList() ?? new List<string>();

            return new DomainViewModel
            {
                Name = domain.Name,
                Ip = domain.Ip,
                HostedAt = domain.HostedAt,
                NameServers = nameServers
            };
        }
    }
}