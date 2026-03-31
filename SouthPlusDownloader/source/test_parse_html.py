from bs4 import BeautifulSoup
with open('debug_html.txt', 'r', encoding='utf-8') as f:
    html = f.read()

soup = BeautifulSoup(html, 'html.parser')
rows = soup.find_all('tr', class_=['tr3 t_one', 'tr3'])
for row in rows:
    a_tag = row.find('a', id=lambda x: x and x.startswith('a_ajax_'))
    doc_id = a_tag.get('id') if a_tag else "N/A"
    text = a_tag.text.strip() if a_tag else "No a_ajax_"
    print(f"{doc_id}: {text}")
