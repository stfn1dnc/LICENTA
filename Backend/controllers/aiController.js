const OpenAI = require('openai');
const openai = new OpenAI({
  apiKey: process.env.AI_KEY
});

async function askAI(req, res, next) {
  try {
    const completion = await openai.chat.completions.create({
      model: "gpt-3.5-turbo",
      messages: [{ role: "user", content: req.body.prompt }]
    });
    res.json({ response: completion.choices[0].message.content });
  } catch (err) {
    next(err); 
  }
}

module.exports = { askAI };
