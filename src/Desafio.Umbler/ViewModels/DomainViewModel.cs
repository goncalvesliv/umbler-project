using System.Collections.Generic;

namespace Desafio.Umbler.ViewModels
{
    public class DomainViewModel
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public string HostedAt { get; set; }
        public List<string> NameServers { get; set; }
    }
}