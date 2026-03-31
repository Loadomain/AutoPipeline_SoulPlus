import urllib.request, re

req = urllib.request.Request('https://south-plus.net/index.php', headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8', errors='ignore')

# Match category IDs: <a href="thread.php?fid-226.html">Comic Market 107</a>
matches = re.findall(r'<a href="thread\.php\?fid-(\d+)\.html">(C(?:omic(?: Market)?)?\s*\d+[^<]*)</a>', html)

for fid, name in matches:
    print(f"[{name} Category] FID: {fid}")
    # fetch the category to see its subboards
    cat_req = urllib.request.Request(f'https://south-plus.net/thread.php?fid={fid}', headers={'User-Agent': 'Mozilla/5.0'})
    cat_html = urllib.request.urlopen(cat_req).read().decode('utf-8', errors='ignore')
    # Match <a href="thread.php?fid-213.html" class="fnamecolor a1"> <b>同人志&CG</b></a>
    sub_matches = re.findall(r'<a href="thread\.php\?fid-(\d+)\.html".*?>\s*<b>(.*?)</b></a>', cat_html)
    for subid, subname in sub_matches:
        if "同人" in subname or "CG" in subname:
            print(f"  --> {subname}: {subid}")
