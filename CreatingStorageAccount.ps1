$resourceGroup = "workshop-storage-rg"
$storageaccountName = "pgsworkshowstorage"
$location = "westeurope"

New-AzureRmResourceGroup -Name $resourceGroup -Location $location

New-AzureRmStorageAccount -ResourceGroupName $resourceGroup `
-Name $storageaccountName `
-Location $location `
-SkuName Standard_LRS `
-Kind StorageV2