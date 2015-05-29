param($installPath, $toolsPath, $package, $project)

#Output value for debugging
Write-Host "Install path: " $installPath
Write-Host "Tools path: " $toolsPath
Write-Host "Package: " $package
Write-Host "Project: " $project


#Web Project or Web site?
if($project.FullName.Contains("http:"))
{
	#It's a website
	Write-Host "This is a website project"
	
	#Remove two levels of folders
	#Install path: C:\Users\Warren\Documents\Visual Studio 2012\Projects\Nuget.Test.Web\packages\LiveLogger.Umbraco.1.0.3
	#NEW path: C:\Users\Warren\Documents\Visual Studio 2012\Projects\Nuget.Test.Web\
	$projectDestinationPath = (Get-Item $installPath).Parent.Parent.FullName + "\"
	Write-Host "Project Destination Path: " $projectDestinationPath
	
}
else
{
	#It's a webapp
	Write-Host "This is a webapp project"
	
	$projectDestinationPath = Split-Path $project.FullName -Parent
	Write-Host "Project Destination Path: " $projectDestinationPath
}


if ($project) {

    #Removes Dummy text file (needed for workaround, as nuget doesnt allow package with no content) 
    $project.ProjectItems | ForEach { if ($_.Name -eq "InstallationDummyFile.txt") { $_.Remove() } }
    Join-Path $projectDestinationPath "InstallationDummyFile.txt" | Remove-Item


  
    #Write to console
    Write-Host "Enabling MVC mode for Umbraco"

	#Output value for debugging
	Write-Host "Project Full Name: " $project.FullName
  
  	# ******Update umbracoSettings.config ******
	$umbConfigFile = Join-Path $projectDestinationPath "config\umbracoSettings.config"
	$umbConfig = [XML](Get-Content $umbConfigFile)
	
	#Output value for debugging
	Write-Host "UmbracoConfigFile: " $umbConfigFile
	
	
 	### Change element with specific attribute value ###
 	$defaultRenderingEngine = $umbConfig.settings.templates.defaultRenderingEngine
	
	#Let's update the value
	$templates = $umbConfig.settings.templates
	
	Write-Host "defaultRenderingEngine (before): " $templates.defaultRenderingEngine
    $templates.defaultRenderingEngine = "Mvc"
    Write-Host "defaultRenderingEngine (after): " $templates.defaultRenderingEngine
	
	
 	
	#Save the config file
    $umbConfig.Save($umbConfigFile)
    
	
	#Write to console
    Write-Host "Updated umbraco.Config with MVC mode & Saved"
	Write-Host "All DONE"
	
}