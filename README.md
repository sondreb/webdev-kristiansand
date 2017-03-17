# webdev-kristiansand
Demo repo for WebDevKristiansand


# Creating a new cluster project

1. Need to add .Domain projects.
2. Need to manually remove Any CPU and force x64.
3. Need to change .NET framework 4.5 to 4.6.1.
4. Need to change create listeners.
5. using Microsoft.ServiceFabric.Services.Remoting.Runtime;
6. Stateful: return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener) };
7. Stateless: return new[] { new ServiceInstanceListener(context => this.CreateServiceRemotingListener(context)) };