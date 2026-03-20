import React, { useState } from 'react'
import ReactDOM from 'react-dom/client'

function DomainSearch() {
  const [domain, setDomain] = useState('')
  const [result, setResult] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const validate = (value) => {
    if (!value || !value.trim()) return 'Digite um domínio.'
    if (!value.includes('.')) return 'Domínio inválido. Ex: umbler.com'
    if (value.startsWith('.') || value.endsWith('.')) return 'Domínio inválido.'
    return null
  }

  const handleSearch = async () => {
    const validationError = validate(domain)
    if (validationError) {
      setError(validationError)
      setResult(null)
      return
    }

    setError('')
    setLoading(true)
    setResult(null)

    try {
      const resp = await fetch(`/api/domain/${domain.trim().toLowerCase()}`, {
        headers: { 'Accept': 'application/json' }
      })

      if (!resp.ok) {
        const msg = await resp.text()
        setError(msg || 'Erro ao consultar domínio.')
        return
      }

      const data = await resp.json()
      setResult(data)
    } catch (e) {
      setError('Erro de conexão.')
    } finally {
      setLoading(false)
    }
  }

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') handleSearch()
  }

  return (
    <div className="dns-wrapper">
      <div className="dns-search-box">
        <input
          className="dns-search-input"
          placeholder="Digite o domínio que deseja pesquisar..."
          type="text"
          value={domain}
          onChange={(e) => setDomain(e.target.value)}
          onKeyDown={handleKeyDown}
        />
        <button className="dns-search-btn" onClick={handleSearch} disabled={loading}>
          {loading ? 'Pesquisando...' : '🔍 Pesquisar'}
        </button>
      </div>

      {error && <div className="dns-error">⚠️ {error}</div>}

      {loading && (
        <div className="dns-loading">
          <div className="dns-spinner"></div>
          Consultando DNS e WHOIS...
        </div>
      )}

      {result && (
        <div className="dns-card">
          <div className="dns-card-header">
            <div className="dns-card-icon">🌐</div>
            <div>
              <h5 className="dns-card-title">{result.name}</h5>
              <p className="dns-card-subtitle">Informações de DNS e registro</p>
            </div>
          </div>
          <div className="dns-card-body">
            <div className="dns-row">
              <span className="dns-row-label">IP Registro A</span>
              <span className="dns-row-value">
                <span className="dns-badge">{result.ip}</span>
              </span>
            </div>
            <div className="dns-row">
              <span className="dns-row-label">Hospedado em</span>
              <span className="dns-row-value">{result.hostedAt}</span>
            </div>
            <div className="dns-row">
              <span className="dns-row-label">Name Servers</span>
              <span className="dns-row-value">
                {result.nameServers && result.nameServers.map((ns, i) => (
                  <span key={i} className="dns-badge">{ns}</span>
                ))}
              </span>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

const root = ReactDOM.createRoot(document.getElementById('whois-results'))
root.render(<DomainSearch />)