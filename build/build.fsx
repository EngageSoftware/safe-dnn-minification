// include Fake lib
#r @"..\packages\FAKE\tools\FakeLib.dll"
open Fake

let tempDir = "./tmp/"
let binDir = "./tmp/bin"
let distDir = "../dist/"
let rootDir = "../"
let srcDir = "../src/"

Target "Clean" (fun _ ->
    CleanDirs [tempDir; distDir]
)

Target "RestorePackages" (fun _ ->
    (srcDir + "packages.config")
      |> RestorePackage (fun p -> 
         { p with 
             OutputPath = (srcDir + "packages")
             ToolPath = (findNuget (rootDir + ".nuget")) })
)

Target "Build" (fun _ ->
    !! (srcDir + "*.csproj")
      |> MSBuildRelease binDir "Build"
      |> Log "Build Output: "
)

Target "Package" (fun _ ->
    !! (srcDir + "*.dnn")
      ++ (rootDir + "LICENSE.htm")
      ++ (rootDir + "CHANGES.htm")
      |> Copy tempDir


    !! (tempDir + "**/*")
      |> Zip tempDir (distDir + "Engage-SafeDnnMinification_Install.zip")
)

// Dependencies
"Clean"
  ==> "RestorePackages"
  ==> "Build"
  ==> "Package"


// start build
RunTargetOrDefault "Package"