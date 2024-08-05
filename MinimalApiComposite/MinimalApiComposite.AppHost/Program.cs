var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("SeqReference", 5341);
var apiService = builder.AddProject<Projects.MinimalAPISample>("apiservice").WithReference(seq);


builder.Build().Run();
