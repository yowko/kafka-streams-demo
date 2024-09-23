using System.Reflection;
using Confluent.Kafka;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;

var config = new StreamConfig<StringSerDes, StringSerDes>();
//必填。我這邊以 application name 為例
config.ApplicationId = Assembly.GetEntryAssembly().GetName().Name;
//必填。我用 docker compose 建立的 localhost 為例
config.BootstrapServers = "localhost:9092";
    
StreamBuilder builder = new StreamBuilder();

// `from` 是來源 topic；將 from 中的 meesage 加上 "from:" 前綴；`to` 是目標 topic
builder.Stream<string, string>("from")
    .MapValues((v) => $"from:{v}")
    .To("to");

Topology t = builder.Build();
KafkaStream stream = new KafkaStream(t, config);

Console.CancelKeyPress += (o, e) => {
    stream.Dispose();
};

await stream.StartAsync();