import re
with open('c107_dump.html', encoding='utf-8') as f:
    html = f.read()

m = re.findall(r'<a.*?href=["\'](thread\.php\?fid-[0-9]+\.html)["\'].*?>(.*?)</a>', html, flags=re.IGNORECASE|re.DOTALL)
for href, text in m:
    print(href, text.strip())
