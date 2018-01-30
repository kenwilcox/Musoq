﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FQL.Service.Client.Helpers
{
    public class RuntimeApi
    {
        private readonly string _address;

        public RuntimeApi(string address)
        {
            _address = address;
        }

        public async Task<Guid> Execute(Guid id)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(_address)
            };
            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"api/runtime/execute?id={id.ToString()}", content);
            var contResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Guid>(contResponse);
        }

        public async Task<ResultTable> Result(Guid id)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(_address)
            };
            var response = await client.GetAsync($"api/runtime/result?id={id.ToString()}");
            return JsonConvert.DeserializeObject<ResultTable>(await response.Content.ReadAsStringAsync());
        }

        public async Task<(bool HasContext, ExecutionStatus Status)> Status(Guid id)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(_address)
            };
            var response = await client.GetAsync($"api/runtime/status?id={id}");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<(bool HasContext, ExecutionStatus Status)>(
                content);
        }
    }
}