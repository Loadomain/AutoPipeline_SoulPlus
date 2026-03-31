import urllib.request, re
try:
    html = urllib.request.urlopen(urllib.request.Request('https://south-plus.net/thread.php?fid=218', headers={'User-Agent': 'Mozilla/5.0'})).read().decode('utf-8', errors='ignore')
    m = re.findall(r'<a[^>]*href="thread\.php\?fid-(\d+)\.html"[^>]*class="fnamecolor[^>]*>\s*(?:<b>)?\s*(.*?)\s*(?:</b>)?\s*</a>', html, flags=re.IGNORECASE)
    print("Found fnamecolor matches:")
    for x in m: print(x)
except Exception as e:
    print(e)
