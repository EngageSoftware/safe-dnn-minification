// include Fake lib
#r @"..\packages\FAKE\tools\FakeLib.dll"
open Fake

RestorePackages()

let tempDir = "./tmp/"
let binDir = "./tmp/bin"
let distDir = "../dist/"
let rootDir = "../"
let srcDir = "../src/"

Target "Clean" (fun _ ->
    CleanDirs [tempDir; distDir]
)

Target "Build" (fun _ ->
    !! (srcDir + "*.csproj")
      |> MSBuildRelease binDir "Build"
      |> Log "Build Output: "
)

Target "Package" (fun _ ->
    !! (tempDir + "**/*")
      ++ (srcDir + "*.dnn")
      ++ (rootDir + "LICENSE.htm")
      ++ (rootDir + "CHANGES.htm")
      |> Zip tempDir (distDir + "Engage-SafeDnnMinification_Install.zip")
)

// Dependencies
"Clean"
  ==> "Build"
  ==> "Package"


// start build
RunTargetOrDefault "Package"