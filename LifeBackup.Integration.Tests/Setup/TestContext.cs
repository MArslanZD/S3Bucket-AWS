using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace LifeBackup.Integration.Tests.Setup
{
    public class TestContext: IAsyncLifetime
    {
        private readonly DockerClient _dockerClient;
        private const string ContainerImageUri = "localstack/localstack";
        private string _ContainerId;

        public TestContext() => _dockerClient = new DockerClientConfiguration(new Uri(DockerApiUri())).CreateClient();
     
        // Will run directly after the Class's Constructor
        public async Task InitializeAsync()
        {
            await PullImage();
            await StartContainer();
        }
        private async Task PullImage()
        {
            await _dockerClient.Images
                .CreateImageAsync(new ImagesCreateParameters
                {
                    FromImage = ContainerImageUri,
                    Tag = "Latest"
                },
                new AuthConfig(),
                new Progress<JSONMessage>());
        }

        private async Task StartContainer()
        {
            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = ContainerImageUri,
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    {
                        "9003",default
                    }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "9003", new List<PortBinding> { new PortBinding { HostPort ="9003"} } }
                    }
                },
                Env = new List<string> { "SERVICES=s3:9003"}
            });
            _ContainerId = response.ID;
            await _dockerClient.Containers.StartContainerAsync(_ContainerId, null);
        }

        private string DockerApiUri() {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            if (isWindows)            
                return "npipe://./pipe/docker_engine";
            else if(isLinux)
                return "unix:/var/run/docker.sock";
            throw new Exception("Unable ro determine the OS...");
        }

        public async Task DisposeAsync()
        {
            if (_ContainerId!=null)            
                await _dockerClient.Containers.KillContainerAsync(_ContainerId, new ContainerKillParameters());             
        }
    }
}