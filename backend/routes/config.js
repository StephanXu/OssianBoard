var express = require('express');
var router = express.Router();

let fs = require('fs');

router.get('/', (req, res, next) => {
    fs.readFile('NVConfig.json', (err, data) => {
        if (err) {
            console.log(err)
            res.status(500).send('err')
        }
        res.contentType('application/json');
        res.status(200).send(data.toString('utf-8'))
    })
});

router.options('/', (req, res, next) => {
    res.contentType('application/json');
    res.status(200).send(JSON.stringify({
        status: 'ok'
    }))
})

router.put('/', (req, res, next) => {
    fs.writeFile('NVConfig.json', JSON.stringify(req.body), () => { })
    res.contentType('application/json');
    res.status(200).send(JSON.stringify({
        status: 'ok'
    }))
})

router.options('/reset', (req, res, next) => {
    res.contentType('application/json');
    res.status(200).send(JSON.stringify({
        status: 'ok'
    }))
})

router.post('/reset', (req, res, next) => {
    let content = fs.readFileSync('NVConfig.default.json');
    fs.writeFile('NVConfig.json', content, () => { })
    res.contentType('application/json');
    res.status(200).send(JSON.stringify({
        status: 'ok'
    }))
})

module.exports = router;