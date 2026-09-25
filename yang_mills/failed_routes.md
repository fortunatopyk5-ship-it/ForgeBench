# Failed / insufficient routes

## Route: Direct Bakry–Emery continuation from strong to weak coupling
Why it looked promising:
Strong-coupling lattice YM has explicit LSI/PI and exponential covariance decay.

Exact failure:
For SU(N), the known curvature constant in Shen–R. Zhu–X. Zhu is
K_S=(N+2)/2-1-8N|beta|(d-1)=N(1/2-8|beta|(d-1)).
In d=4 this requires |beta|<1/48. The Wilson continuum trajectory requires inverse bare coupling beta->infinity. Thus the sufficient curvature condition moves in the wrong direction and becomes negative.

Can it be repaired?
Only by a genuinely multiscale/renormalized inequality; not by pushing the same global curvature estimate.

---

## Route: Positive finite-volume/fixed-cutoff gap + continuity in beta
Why it looked promising:
Finite-volume transfer matrices can have isolated positive gaps at every finite beta.

Exact failure:
Pointwise positivity has no uniform content. Toy family H_beta=diag(0,e^{-beta}) has positive gap for every beta but gap->0. Likewise a volume gap can close as L->infinity.

Can it be repaired?
Need an explicit quantitative lower bound with the correct beta, a, and L scaling.

---

## Route: No phase transition from strong to weak coupling implies gap persists
Why it looked promising:
Strong coupling has a rigorous gap and analyticity.

Exact failure:
Even an analytic strictly positive function on every finite beta can tend to zero as beta->infinity. The continuum limit occurs at beta=infinity, so absence of finite-beta singularities is insufficient.

Can it be repaired?
Need quantitative asymptotic control of the gap relative to a(beta).

---

## Route: Lambda_YM>0 implies Delta>0
Why it looked promising:
Dimensional transmutation produces the only natural IR scale.

Exact failure:
Scale generation does not exclude a zero-energy continuum in the gauge-invariant spectrum. The missing theorem is C_G>0 in m_gap=C_G Lambda_YM.

Can it be repaired?
Prove YM-IR-1 or an equivalent transfer/correlation inequality.

---

## Route: Wilson area law implies the full mass gap
Why it looked promising:
Both are associated physically with confinement.

Exact failure:
Area law controls a nonlocal line operator/static-charge sector; the Clay gap is a statement about the full vacuum-sector Hamiltonian spectrum. No general implication follows without extra locality/positivity/density assumptions.

Can it be repaired?
Use area law only as one ingredient; separately control a dense local gauge-invariant algebra.

---

## Route: Poincare/LSI alone implies spatial mass gap
Why it looked promising:
PI gives exponential decay for the auxiliary reversible Markov semigroup.

Exact failure:
That is decay in stochastic-quantization time, not Euclidean spatial separation. A second quasi-locality/finite-propagation estimate is required. In the known strong-coupling proof this comes from commutator bounds [nabla_e,L].

Can it be repaired?
Target the pair (lambda_a,v_a), not lambda_a alone.


---

## Route: Global stochastic-quantization Poincare gap as the final target
Why it looked promising:
Together with locality it gives spatial mixing (YM-SEMIGROUP-1).

Exact failure / overstrength:
The auxiliary Markov generator can possess very slow global or topological modes that are not the physical Hamiltonian excitations controlling local glueball correlations. Thus a volume-uniform global Markov PI could fail even when the physical theory is locally gapped.

Can it be repaired?
Use conditional/block Poincare inequalities, restricted local-observable decay, or RG landing directly into a local polymer-mixing domain.
