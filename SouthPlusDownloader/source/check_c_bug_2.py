import urllib.request, re

try:
    req = urllib.request.Request('https://south-plus.net/index.php', headers={'User-Agent': 'Mozilla/5.0'})
    html = urllib.request.urlopen(req).read().decode('utf-8', errors='ignore')
    
    matches = re.finditer(r'<a[^>]*href="thread\.php\?fid-(\d+)\.html"[^>]*>\s*(C(?:omic(?: Market)?)?\s*\d+[^<]*)\s*</a>', html, re.IGNORECASE)
    for m in matches:
        print(f"[{m.group(1)}] {m.group(2)}")
except Exception as e:
    print(e)
