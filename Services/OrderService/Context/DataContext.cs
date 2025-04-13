using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient.Protocol;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Models;

namespace OrderService.Context
{
    public class DataContext
    {

        private readonly IMongoDatabase database; 
        private readonly IMongoCollection<Order> orderCollection;

        public DataContext(IOptions<MongoDbSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            database = client.GetDatabase(options.Value.DatabaseName);
            orderCollection = database.GetCollection<Order>(options.Value.OrderCollection);
        }

        public IMongoCollection<Order> Orders => orderCollection;
    }
}