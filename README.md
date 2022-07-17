# TableStoreRepository


### Pushing packages to github
dotnet pack --configuration Release

dotnet nuget push .bin\release\dnkh.TableStoreRepository.0.0.1.nupkg --source https://nuget.pkg.github.com/dnkh/index.json --api-key <TOKEN / Personal access token from gitaccount>
