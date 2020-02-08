# nCoV
### Brief
#### This Project will put the number of the website("https://ncov.dxy.cn/ncovh5/view/pneumonia_peopleapp") to your disk  

### Notice
- This software will download the page once every 10 seconds
- This is within the law in China 
- Please check the law in your country.

### Default Info Store Path
- F:\nCov.txt
- you can change "~/nCov/FileHandle.cs" line 22 to change Store Path

### Proxy Setting 
- Due to chinese special network, you can choose with or not with proxy to use this project 
- this project default <B>NOT WITH PROXY</B> 
- You can delete the parameter of "Class HttpClient" in "~/nCov/webSocket.cs" line 23  
such as <b>using (HttpClient hc = new HttpClient())</b> to use your system proxy
