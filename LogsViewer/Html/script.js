
let lastTr = document.createElement('tr');

function LogLevelColor(level) {
    switch (level) {
        default: return 'table-secondary';
        case 'Fatal': return 'table-dark';
        case 'Error': return 'table-danger';
        case 'Warning': return 'table-warning';
        case 'Verbose': return 'table-default';
    }
}
function TaskColor(task) {
    switch (task.status) {
        default: return 'table-danger';
        case 'Task delayed: {Time} {Reason}': return 'table-warning';
        case 'Task complete': return 'table-default';
    }
}

function ToJson(key, value) {
    if (key == '@t' || key == '@mt' || key == '@l' || key == '@x') {
        return undefined
    }
    if (key == 'task' && typeof value == 'string') {
        return JSON.parse(value)
    }
    if (key == 'Url' && typeof value == 'string') {
        return value.length < 150 ? value : value.substring(0, 150) + '...'
    }
    return value
}

async function LoadLogs(uid) {
    const response = await fetch("/task/" + uid)
    const lines = await response.json()
    const stats = {}

    logrows.innerHTML = ''

    lines.forEach(line => {
        const data = JSON.parse(line)
        const tr = document.createElement('tr')
        const td = document.createElement('td')

        const ex = data['@x'] ? `<pre>\n\n${data['@x']}</pre>` : ''
        const ts = data['@t'].replace('T', ' ').replace('Z', '')
        td.innerHTML = `${ts} &mdash; ${data['@mt']}<br/>${JSON.stringify(data, ToJson, 3)}${ex}`

        tr.className = LogLevelColor(data['@l'])
        tr.appendChild(td)
        logrows.appendChild(tr)

        const lvl = data['@l'] ?? 'Info'
        stats[lvl] = 1 + (stats[lvl] ?? 0)
    })

    logstats.innerHTML = Object.entries(stats).map(x => x[0] + ': ' + x[1]).join(', ')
}

fetch("/task-list").then(async x => {
    const runs = await x.json();
    const stats = {}

    runs.forEach(run => {
        const ts = new Date(run.start)

        const tr = document.createElement('tr');
        tr.className = 'row-run table-dark'
        tr.innerHTML = `<td colspan="4">${ts.toLocaleDateString()} ${ts.toLocaleTimeString()}</td>`
        tasks.appendChild(tr)

        run.tasks.forEach(task => {
            const tr = document.createElement('tr');
            tr.className = 'row-task ' + TaskColor(task)
            tr.taskColor = TaskColor(task)
            tr.innerHTML = `<td>${task.threadId}</td><td>${task.type}</td><td>${task.status}</td><td>${task.lines}</td>`
            tasks.appendChild(tr)

            tr.onclick = () => {
                lastTr.classList.remove('table-primary');
                lastTr.classList.add(lastTr.taskColor);
                lastTr = tr;                
                lastTr.classList.remove(lastTr.taskColor);
                lastTr.classList.add('table-primary');
                LoadLogs(task.uid)
            }

            stats[tr.taskColor] = 1 + (stats[tr.taskColor] ?? 0)
        })
    })
    taskstats.innerHTML = `Complete: ${stats['table-default'] ?? 0}, Delay: ${stats['table-warning'] ?? 0}, Fail: ${stats['table-danger'] ?? 0}`
})
