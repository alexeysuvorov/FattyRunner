﻿namespace FattyRunner.Engine

module ReflectionHelper = 
    open System
    open System.Reflection
    
    let instantiate (t : Type) (cts: Map<string,obj> option) = 
        match cts with
        | Some(cts) -> 
            Activator.CreateInstance(t, [| cts :> obj |])
        | None -> 
            Activator.CreateInstance(t, null)

module TestRunnerEngine = 
    open System
    open System.Diagnostics

    let prepareEnvironment cfg = ()
    let shutdownEnvironment cfg = ()
    let beforeStart config = ()
    let afterEnd config = ()
    
    let executeStep n x = 
        let rec exec' f n =
            if n = 0 then ()
            else do f() |> ignore  
                 exec' f (n-1)

        let sw = Stopwatch.StartNew();
        do exec' x n
        do sw.Stop()
        sw.ElapsedMilliseconds
    
    let runTest (cfg:EnvironmentConfiguration) (t : Test) = 
        do prepareEnvironment cfg
        let instance = ReflectionHelper.instantiate t.Reference.Type cfg.Context
        
        let decoratedExecute n x = 
            beforeStart cfg
            let res = executeStep n x
            afterEnd cfg
            res
        
        let step = 
            match t.Configuration.ProgressiveStep with
            | 0u -> 1 
            | x when x > t.Configuration.Count -> 
                int t.Configuration.Count
            | x -> int x

        let count = int t.Configuration.Count
        
        let fu() = t.Reference.Run.Invoke(instance, null)

        match t.Configuration.WarmUp with
        | 0u -> () 
        | x -> seq {1..int x } |> Seq.iter (fun _-> fu() |> ignore)

        do System.Threading.Thread.Sleep(5)

        let timings = 
            seq { step..step..count }
            |> Seq.map (fun x -> (x, decoratedExecute x fu))
            |> Seq.toArray
        
        do shutdownEnvironment cfg
        timings
    
    let run (tests : Test list) cfg = tests |> Seq.map (runTest cfg)