import re
html = open('c107_dump.html', encoding='utf-8').read()
m = re.findall(r'href="(.*?)"', html)
for x in m:
    if 'read' in x or 'thread' in x or 'fid' in x:
        print(x)
