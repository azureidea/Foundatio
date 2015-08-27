﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Caching;
using Foundatio.Jobs;
using Foundatio.Lock;
using Foundatio.Messaging;
using Xunit;

namespace Foundatio.Tests.Jobs {
    public class WithLockingJob : JobBase {
        private readonly ILockProvider _locker = new CacheLockProvider(new InMemoryCacheClient(), new InMemoryMessageBus());
        public int RunCount { get; set; }

        protected override Task<IDisposable> GetJobLockAsync(){
            return _locker.AcquireLockAsync("WithLockingJob", TimeSpan.FromSeconds(1), TimeSpan.Zero);
        }

        protected override async Task<JobResult> RunInternalAsync(CancellationToken token) {
            RunCount++;

            Thread.Sleep(150);
            Assert.True(await _locker.IsLockedAsync("WithLockingJob"));

            return JobResult.Success;
        }
    }
}
