﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using Parallelator.Common;
using Parallelator.Loaders.Deserializing;

namespace Parallelator.Benchmark.Benchmarks
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 1, warmupCount: 0, targetCount: 3)]
    [MemoryDiagnoser]
    [RankColumn(NumeralSystem.Stars)]
    [OrderProvider(SummaryOrderPolicy.SlowestToFastest)]
    public class DeserializingLoaderBenchmarks
    {
        private Uri[] _uris;

        [Params(255)]
        public int NumOfEntries { get; set; }

        [Params(512)]
        public int ResponseDelay { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _uris = ApiUriBuilder.GenerateUris(ResponseDelay, NumOfEntries);
        }

        [Benchmark(Description = "Loading one by one")]
        public async Task<IEnumerable<DummyData>> SequentialDeserializingAsync()
        {
            var downloader = new SequentialDeserializingLoader();
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark(Description = "Always keeping n tasks concurrently")]
        public async Task<IEnumerable<DummyData>> TaskContinuousBatchDeserializingLoaderAsync()
        {
            var downloader = new TaskContinuousBatchDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark(Description = "Awaiting batches (groups) of tasks")]
        public async Task<IEnumerable<DummyData>> TaskSeqBatchDeserializingLoaderAsync()
        {
            var downloader = new TaskSeqBatchDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark(Description = "Producer-Consumer with semaphore")]
        public async Task<IEnumerable<DummyData>> TaskEnumWithSemaphoreDeserializingLoaderLoaderAsync()
        {
            var downloader = new TaskEnumWithSemaphoreDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark(Description = "TPL Dataflow obtaining string")]
        public async Task<IEnumerable<DummyData>> DataFlowDeserializingLoaderAsync()
        {
            var downloader = new DataFlowDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark(Description = "TPL Dataflow obtaining stream")]
        public async Task<IEnumerable<DummyData>> DataFlowStreamDeserializingLoaderAsync()
        {
            var downloader = new DataFlowStreamDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark(Description = "Parallel.Invoke with degree of parallelism")]
        public async Task<IEnumerable<DummyData>> ParallelInvokeDeserializingLoaderAsync()
        {
            var downloader = new ParallelInvokeDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark(Description = "ForEachAsync with concurrent bag as storage")]
        public async Task<IEnumerable<DummyData>> ParallelForEachDeserializingLoaderAsync()
        {
            var downloader = new ParallelForEachDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark(Description = "Producer-Consumer 1:1, producer->task, consumer->await & deserializes")]
        public async Task<IEnumerable<DummyData>> ProducerTaskConsumerAwaitsDeserializingLoaderAsync()
        {
            using (var downloader = new ProducerTaskConsumerAwaitsDeserializingLoader(Constants.MaxConcurrency))
            {
                return await downloader.LoadAsync(_uris);
            }
        }

        [Benchmark(Description = "Producer-Consumer 1:1, producer->string, consumer->deserializes")]
        public async Task<IEnumerable<DummyData>> Producer1Consumer1DeserializingLoaderAsync()
        {
            using (var downloader = new Producer1Consumer1DeserializingLoader(Constants.MaxConcurrency))
            {
                return await downloader.LoadAsync(_uris);
            }
        }

        //[Benchmark(Description = "Producer-Consumer with Threads")]
        public async Task<IEnumerable<DummyData>> ProducerConsumerThreadsDeserializingLoaderAsync()
        {
            using (var downloader = new ProducerConsumerThreadsDeserializingLoader(Constants.MaxConcurrency))
            {
                return await downloader.LoadAsync(_uris);
            }
        }
    }
}
