FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY CourseWork.Entities/CourseWork.Entities.csproj CourseWork.Entities/
COPY CourseWork.Application/CourseWork.Application.csproj CourseWork.Application/
COPY CourseWork.DataAccess/CourseWork.DataAccess.csproj CourseWork.DataAccess/
COPY CourseWork.API/CourseWork.API.csproj CourseWork.API/

RUN dotnet restore CourseWork.API/CourseWork.API.csproj

COPY CourseWork.Entities/ CourseWork.Entities/
COPY CourseWork.Application/ CourseWork.Application/
COPY CourseWork.DataAccess/ CourseWork.DataAccess/
COPY CourseWork.API/ CourseWork.API/

RUN dotnet publish CourseWork.API/CourseWork.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CourseWork.API.dll"]
