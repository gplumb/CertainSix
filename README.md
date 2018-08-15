# CertainSix ![Build Status](https://api.travis-ci.org/gplumb/CertainSix.svg?branch=master)

##Shipping Container Spoilage
Build an API to track shipping container temperature sensors to extract useful insights.

### Stack
* DotNet Core 2.1 WebAPI (a modern, clean, cross-platform framework that promotes good development practices, such as DI and separation of concerns)
* EF Core (InMemory and MS SQL Server provider)
* PowerShell for data generation (cross platform)

### Tools Used
* Swagger UI (Development / DEBUG mode only), which provides a nice way to navigate the api spec and exercise its implementation from within a browser (avoiding the need for external tools like _Postman_)
* Travis CI for on-commit unit testing
* Open API Tools for generating api view models and controller signatures from the api spec [https://github.com/OpenAPITools/openapi-generator](https://github.com/OpenAPITools/openapi-generator)
 
### How to run locally
Simply clone the repository, and run the solution in the _Debug_ configuration in Visual Studio (2017 or Mac). Storage defaults to an in-memory (transient) data store.

To use an MS SQL database:

 * Edit _appSettings.json_ and update the `SqlConnection` property so that it points to your database server
 * Switch the project to the _Release_ configuration
 * Using the _Package Manager_, execute the command `Update-Database`

### Known Issues
* As it stands, there is no authentication required to use this api (per the spec)
* The ETag provided in the `\trips\{tripId}` endpoint isn't actually enforced (per the spec)

### Generate test data
In the `.\Scripts` folder, there is a PowerShell script that can be used to generate container payloads for the `\trips\{tripId}/containers` endpoint.

The script generates an oscillating range of temperature sensor data, the frequency, speed and range of which may all be optionally configured.

#### Basic usage
To execute, run the following command: `powershell .\GenerateData.ps1` (which will generate a file called `output.json`, containing enough data to simulate 5 day's worth of data collection).

#### Advanced usage
The script may be executed with one or more optional configuration arguments. Here's an example that uses 2 optional arguments:
`powershell .\GenerateData.ps1 -x y -x1 y2`

Where:

 * `x` is the name of an optional setting
 * `y` is the value of `x`
 * `x1` is the name of another optional setting
 * `y1` is the value of `x1`
 * etc...

The arguments available to this script are:

<table>
 <tr>
  <td>Argument Name</td><td>Type</td><td>Usage</td>
 </tr>
 <tr><td><i>fileName</i></td><td><i>string</i></td><td>Where to write the resulting json</td></tr>

  <tr><td><i>containerName</i></td><td><i>string</i></td><td>The name of the container</td></tr>

  <tr><td><i>productCount</i></td><td><i>int</i></td><td>The amount of product in the container</td></tr>

  <tr><td><i>minTemp</i></td><td><i>int</i></td><td>Minimum temperature for generated samples</td></tr>

  <tr><td><i>maxTemp</i></td><td><i>int</i></td><td>Maximum temperature for generated samples</td></tr>

  <tr><td><i>interval</i></td><td><i>float</i></td><td>The temperature variance, per sample</td></tr>

  <tr><td><i>dropRate</i></td><td><i>float</i></td><td>Percentage of samples to drop (To simulate sensor inaccuracy)
</td></tr>

  <tr><td><i>sampleRate</i></td><td><i>int</i></td><td>The frequency of temperature sampling (in seconds)</td></tr>

  <tr><td><i>days</i></td><td><i>int</i></td><td>The number of day's worth of data to generate</td></tr>
</table>
