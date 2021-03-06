﻿namespace FattyRunner.Engine

module internal ReflectionHelper = 
    open System
    open System.Reflection
    
    let contextType = typeof<FattyRunner.Interfaces.ExternalContext>

    let implements (implemented:Type) (implementor:Type) =
        implemented.IsAssignableFrom implementor

    let implementsDispose t = implements typeof<IDisposable> t

    let getDisposeReference (t: Type) =
        if implements typeof<IDisposable> t then
            Some(typeof<IDisposable>.GetMethod("Dispose"))
        else None

    let instantiate (t : Type) (cts: FattyRunner.Interfaces.ExternalContext) = 
        let parameteredCtor = t.GetConstructor([|contextType|])
        if null = parameteredCtor then Activator.CreateInstance(t, null) 
        else Activator.CreateInstance(t, [| cts :> obj |])

    let callOption instance (f:System.Reflection.MethodInfo option) =
        match f with
        | Some(f) -> f.Invoke(instance,null) |> ignore
        | _ -> ()

    let markedByAttr t (m:MethodInfo) =
            m.CustomAttributes |> Seq.exists (fun a -> a.AttributeType = t)

    let tryLoadAssembly (s:string) =
        try 
            let testAssembly = System.Reflection.AssemblyName.GetAssemblyName(s)
            Some(System.Reflection.Assembly.LoadFile(s))
        with
        | :? System.IO.FileNotFoundException -> None
        | :? System.BadImageFormatException -> None
        | :? System.IO.FileLoadException -> None

            