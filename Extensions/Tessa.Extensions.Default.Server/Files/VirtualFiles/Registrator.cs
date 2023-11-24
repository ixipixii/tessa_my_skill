﻿using Unity;
using Unity.Lifetime;
using Tessa.Extensions.Default.Server.Files.VirtualFiles.Compilation;

namespace Tessa.Extensions.Default.Server.Files.VirtualFiles
{
    [Registrator]
    public sealed class Registrator : RegistratorBase
    {
        public override void RegisterUnity()
        {
            this.UnityContainer
                .RegisterType<IKrVirtualFileManager, KrVirtualFileManager>(new ContainerControlledLifetimeManager())
                .RegisterType<IKrVirtualFileCache, KrVirtualFileCache>(new ContainerControlledLifetimeManager())

                .RegisterType<IKrVirtualFileCompiler, KrVirtualFileCompiler>(new ContainerControlledLifetimeManager())
                .RegisterType<IKrVirtualFileCompilationCache, KrVirtualFileCompilationCache>(new ContainerControlledLifetimeManager())
                ;
        }

    }
}
