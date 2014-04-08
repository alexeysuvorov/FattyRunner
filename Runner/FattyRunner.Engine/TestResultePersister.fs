﻿namespace FattyRunner.Engine

module TestResultePersister =
    open System

    type TestResultContainer = 
        { Tests: TestResult seq }
    
    let serialize (tests: TestResult seq) =
        let container = { Tests = tests }
        ()
