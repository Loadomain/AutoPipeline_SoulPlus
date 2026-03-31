import json, urllib.request, re
try:
    with open('credentials.json', 'r', encoding='utf-8') as f:
        cookie = json.load(f).get('Cookie', '')
        
    req = urllib.request.Request('https://south-plus.net/thread.php?fid=226', headers={'User-Agent': 'Mozilla/5.0', 'Cookie': cookie})
    html = urllib.request.urlopen(req).read().decode('utf-8', errors='ignore')
    
    with open('c107_dump.html', 'w', encoding='utf-8') as fw:
        fw.write(html)
        
    m = re.findall(r'<a[^>]*>(.*?)</a>', html, flags=re.IGNORECASE|re.DOTALL)
    print("Tags with 同人 or CG:")
    for x in m:
        if '同人' in x or 'CG' in x: print(x.strip())
except Exception as e:
    print(e)
