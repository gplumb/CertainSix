# Script to generate oscillating TemperatureRecords within a given range
# The frequency of sampling, temperature variance and sample drop rate are all configurable

# Optional parameters with sensible defaults
param([string]$fileName      = "output.json",    # Where to write the resulting json
      [string]$containerName = "container",      # The name of the container
      [Int32] $productCount  = 10,               # The amount of product in the container

      [Int32] $minTemp       = -10,              # Minimum temperature for generated samples
      [Int32] $maxTemp       = 10,               # Maximum temperature for generated samples
      [float] $interval      = 0.1,              # The temperature variance, per sample
      [float] $dropRate      = 0.37,             # Percentage of samples to drop (To simulate sensor inaccuracy)

      [Int32] $sampleRate    = 1,                # The frequency of temperature sampling (in seconds)
      [Int32] $days          = 5)                # The number of day's worth of data to generate
      

# Validate arguments
If ($sampleRate -lt -1)
{
    throw new Exception("Sample interval must be positive")
}

If ($days -lt -1)
{
    throw "Days must be positive!"
}

If ($dropRate -lt -1 -or $dropRate -gt 100)
{
    throw "Drop rate must be positive percentage (0-100)!"
}

If ($productCount -lt -1)
{
    throw "Product count rate must be positive!"
}

If($interval -eq 0)
{
    throw "Temperature interval must be non-zero!"
}

If ($interval -le $minTemp -or $interval -ge $maxTemp)
{
    throw "Temperature interval must be within temperature bounds (minTemp < interval < maxTemp"
}


# Variable setup

# 60 seconds * 60 minutes * 24 hours * X days
[Int32]$sampleTotal = (60 * 60 * 24 * $days) / $sampleRate

# Drop 1 in every X samples
[float]$sampleTemp = $sampleTotal / [float]100 * $dropRate
[Int32]$sampleDrop = $sampleTemp

# Pick a temperature starting rate
[Int32]$medianTemp = ($maxTemp + $minTemp) / 2

[float]$dropCount = -1
[float]$temp = $medianTemp
[string]$tempText = ""
#$date = Get-Date
$date = [System.DateTime]::UtcNow;

# Write JSON header (courtesy of .net)
$writer = New-Object System.IO.StreamWriter $fileName, $false

$writer.AutoFlush = true
$writer.WriteLine("{")
$writer.WriteLine("  ""id"" : """ + $containerName + """,")
$writer.WriteLine("  ""productCount"" : """ + $productCount.ToString() + """,")
$writer.WriteLine("  ""measurements"" : [")

# Let the user know we've started
Write-Host "Generating data..." 

# Main loop (generate the data and write it to the file)
For ([float]$x=0; $x -lt $sampleTotal; $x++)
{
    $dropCount++

    # Drop this sample?
    If ($dropCount -eq $sampleDrop)
    {
        $dropCount = 0
        continue
    }

    # Write sample separator to the output (from previous iteration)
    If ($x -gt 0)
    {
        $writer.WriteLine(",")
    }

    # Calculate temperate
    $temp = $temp + $interval

    # Did we cross a threshold?
    If(($interval -gt 0 -and $temp -gt $maxTemp) -or ($interval -lt 0 -and $temp -lt $minTemp))
    {
        # Yes, reverse the interval and get a new value
        $interval = -$interval
        $temp = $temp + $interval
    }

    # Prepare output values
    $date = $date.AddSeconds($sampleRate)
    $tempText = [string]::Format("{0:0.00}", $temp)

    # Write this entry
    $writer.WriteLine("    {")
    $writer.WriteLine("      ""time"" : """ + $date.ToString("o") + """,")
    $writer.WriteLine("      ""value"" : " + $tempText)
    $writer.Write("    }")
}


# Write JSON footer
$writer.WriteLine("")
$writer.WriteLine("  ]")
$writer.WriteLine("}")
$writer.Close()
$writer.Dispose()

# Let the user know we've finished
Write-Host "Written to:" $fileName