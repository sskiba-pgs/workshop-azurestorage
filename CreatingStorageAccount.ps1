$resourceGroup = "workshop-storage-rg"
$location = "westeurope"

New-AzureRmResourceGroup -Name $resourceGroup -Location $location

New-AzureRmStorageAccount -ResourceGroupName $resourceGroup `
-Name "pgsworkshowstorage" `
-Location $location `
-SkuName Standard_LRS `
-Kind StorageV2