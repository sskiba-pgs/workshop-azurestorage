$resourceGroup = "workshop-storage-rg"
$storageaccountName = "pgsworkshowstorage"
$containerName = "workshopblobs"

$storageaccount = Get-AzureRmStorageAccount -Name $storageaccountName -ResourceGroupName $resourceGroup
$ctx = $storageaccount.Context
        
New-AzureStorageContainer -Name $containerName -Context $ctx -Permission blob

Set-AzureStorageBlobContent -File "C:\Workshop\kitten1.jpg" `
  -Container $containerName `
  -Blob "kitten001.jpg" `
  -Context $ctx 