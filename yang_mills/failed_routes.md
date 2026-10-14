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


---

## Route: Track only g_k or beta_k through the crossover
Why it looked promising:
Asymptotic freedom suggests a monotone running coupling toward strong coupling.

Exact failure:
Generated operators can grow or approach a non-small stationary remainder even while the scalar coupling moves monotonically. Explicit toy systems:
R(g,r)=(g+1,r+1),
and
R(g,r)=(g+delta,rho r+h) with h/(1-rho) above the desired KP radius.

Can it be repaired?
Yes, but only with a coupled invariant-tube estimate controlling the full remainder norm.

---

## Route: Coarse measure enters D_KP, therefore microscopic observables cluster
Why it looked promising:
Exact RG preserves the partition function and the final coarse measure is strongly mixing.

Exact failure:
For a coarse variable V,
Cov(F,G)=Cov(E[F|V],E[G|V])+E[Cov(F,G|V)].
D_KP controls the first term only. A hidden/conditional sector can retain long-range correlations even when the visible coarse measure is product.

Can it be repaired?
Track sources/insertions through the RG and prove shell-by-shell quasi-locality plus mixed-source decay (YM-RG-SOURCE-1).

---

## Route: "Balaban UV stability" directly supplies K_match
Why it looked promising:
Balaban's program constructs RG effective actions and proves the stated UV stability of 4D pure gauge theory.

Exact failure:
UV stability, as such, does not identify a closed/bounded matching tube at fixed g_match in the new global group-valued Banach norm, nor does it prove the matching-scale relation to Lambda_YM, thermodynamic/IR control, or the complete continuum OS construction.

Can it be repaired?
Prove YM-MATCH-EXTRACT-1 by extracting explicit bounds from the Balaban effective-density representation and translating them into crossover_spec.md.


---

## Route: Treat the chosen KP domain as if it characterized all gapped/mixing theories
Why it looked promising:
KP gives a clean rigorous endpoint with explicit exponential decay.

Exact failure:
A measure can be exactly product across blocks and therefore have zero connected correlations, while a poorly chosen product-Haar activity representation has arbitrarily large one-block activities and violates the fixed KP smallness condition.

Can it be repaired?
Interpret D_KP only as one sufficient landing chart. If the invariant tube approaches strong mixing but Q_KP remains large because of local weights, re-factor the reference product measure/local counterterms before declaring the route blocked.

---

## Route: Start source control only at the crossover matching scale
Exact failure:
Microscopic observables have already passed through a cutoff-dependent number of UV integrations before K_match. Losing quasi-locality there cannot be repaired by a perfect finite crossover.

Can it be repaired?
Add YM-UV-SOURCE-MATCH-1 or strengthen YM-MATCH-EXTRACT-1 to include source kernels.


---

## Route: Extract Balaban matching output directly into the old global all-field C^p tube
Why it looked promising:
The link group G is compact, so every fixed smooth local function has finite C^p sup norm.

Exact failure:
Balaban's actual UV estimates are not stated as global derivatives of one recombined interaction. They use restricted small-field analytic domains plus a domain/characteristic decomposition and separate large-field activities. Compactness of G does not turn probabilistic/activity suppression into pointwise smallness, nor does it control derivatives across sector boundaries.

Can it be repaired?
Use a domain-indexed regulated activity topology native to the available estimates. A later bounded embedding into a global C^p chart would be an additional theorem, not an assumption.

---

## Route: Treat exact fiber-integration derivative identities as derivatives of the full RG
Why it looked promising:
For E(Phi)=-log int Q exp(-S_Phi), D E is a conditional expectation and D^2 E is minus a conditional covariance.

Exact failure:
The full map is R=L o S o E. Localization/extraction and rescaling contribute their own operator norms and, if L is nonlinear, D^2 L terms. No full-R C^2 theorem in the crossover Banach topology has been proved.

Can it be repaired?
Prove the E/S/L mapping properties and then use YM-RG-CHAINRULE-1.


---

## Route: Treat the native l-infinity coefficient product as the actual effective-density space
Why it looked promising:
Balaban local activity bounds have exactly the pointwise exponential form that fits a weighted l-infinity product.

Exact failure:
The full density is a sum over admissible multiscale histories with characteristic factors and T-operations. Arbitrary coefficient families need not satisfy admissibility/gluing/reconstruction, and sup over histories does not control their sum.

Can it be repaired?
Use a closed linear compatibility layer plus a nonlinear reconstruction graph, and add a source-derived history summability/overlap norm.

---

## Route: Assume characteristic functions make complete histories disjoint
Why it looked promising:
Small/large-field decompositions often arise from partitions of unity.

Exact failure:
No primary/source theorem was extracted showing that complete CMP119 multiscale histories are pairwise disjoint at fixed field, or that their overlap multiplicity is cutoff-uniform. The density formula explicitly sums histories, and S_j summation is included in T_k.

Can it be repaired?
Extract exact disjointness/overlap from the primary source, or prove a weighted history-summability bound.

---

## Route: Use a single raw common analytic domain across all historical scales
Why it looked promising:
A compact matching interval in the current coupling suggests a positive analytic radius.

Exact failure:
The effective density retains terms generated at earlier UV scales; their source analytic radii depend on g_j and can shrink as g_j->0 when the cutoff is removed. The infimum of raw radii can therefore vanish.

Can it be repaired?
Normalize each source analytic domain by a scale/coupling-dependent chart into a fixed reference domain and prove uniform chart/RG distortion.

---

## Route: Model the Balaban coupling by one scalar c=1/g^2
Why it looked promising:
The bulk classical action has coefficient 1/g^2.

Exact failure:
CMP119 uses a localized coupling c_j(x) and a cutoff/domain function phi_j(x). Boundary/history geometry produces nonconstant coupling profiles.

Can it be repaired?
Use scalar bulk c plus a coupling-profile coordinate q(x)=c(x)-c.


---

## Route: Require absolute summability over complete global histories
Why it looked promising:
It turns reconstruction into an ordinary Banach-valued series with a simple global H_0.

Exact failure:
Even an exact product hard partition with one binary history choice per distant block has 2^N global histories of unit sup norm while fixed local normalized observables remain independent of N. The norm destroys exact remote partition cancellation before it can occur.

Can it be repaired?
Do not norm complete histories globally. First factor/resum into connected local components and use anchored local activity norms.

---

## Route: Use raw unnormalized density as the volume-independent target D
Why it looked promising:
It would make reconstruction literal.

Exact failure:
The Gibbs/effective density contains an extensive vacuum/free-energy factor; source UV-stability bounds naturally scale exponentially with total volume.

Can it be repaired?
Quotient additive constants or retain only vacuum energy density, and norm the connected relative log-density/local activities. Use Z(J)/Z(0) or W(J) only as normalized downstream observables.


---

## Route: Treat the old coordinatewise h_1 as the full derivative norm
Exact failure:
A sum of coordinate Lipschitz constants does not automatically represent the operator norm between anchored polymer spaces, and the previous local reconstruction was not even typed when different output coordinates lived in different spaces.

Repair:
Use the direct-sum local target and the selector-based weighted Schur theorem YM-HISTORY-JACOBIAN-1.

---

## Route: Infer C^1 merely from the phrase "analytic extension"
Exact failure:
Source analyticity in field/source variables does not automatically mean analyticity of the history-resummation map with respect to arbitrary anchored activity coefficients, nor does it supply a cutoff-independent Cauchy radius.

Repair:
Separate field/source analyticity from coefficient analyticity. For coefficient C^1 require a strict cluster convergence ball; for source derivatives require a normalized inner/outer analytic-domain collar.

---

## Route: Demand one identical raw H^infinity field domain at every scale before any coefficient calculus
Exact failure:
A fixed Banach product may have different Banach spaces in different scale/polymer coordinates. Identical raw domains are not necessary for activity-coefficient C^1.

Repair:
Use a scale-graded anchored product. Retain the normalized-chart problem for field/source differentiation and RG chart transitions, where it is genuinely necessary.


---

## Route: Read Balaban's "sufficiently small" as the Banach convergence radius r_*
Exact failure:
The source smallness conditions are used to construct and bound the particular RG-generated pre-exponentiation activity F. They do not directly state a ball theorem for arbitrary complex activity families in our anchored norm.

Repair:
Construct an independent explicit generalized-KP radius
r_*=a exp[-16a-2theta],
then use the source F bound and p_0(g)->infinity only to place the actual Yang-Mills activity strictly inside this ball.

---

## Route: Require the extra spatial Schur reserve to claim any C^1 result
Exact failure:
The off-diagonal Schur estimate is stronger than bounded C^1. Banach-valued holomorphy on a larger input ball already yields a uniform derivative operator norm into B_out(theta).

Repair:
Separate:
- bounded C^1: closed by KP normal convergence + Banach Cauchy;
- quasi-local Jacobian: requires the additional reserve theta-mu_out-nu in the geometric-summability regime.


---

## Route: Infer native incompatibility iff from the word "polymer expansion"
Failure:
CMP122-II p.388 shows that actual X'_p are components of a special decomposition and hence have disjoint cube carriers, but the extracted primary text does not state the converse that every pairwise carrier-disjoint family is admissible under the prime.

Consequence:
C_inc=1 and c_stitch=2 are proved only for the explicitly defined canonical carrier-overlap gas, not yet for native (1.90).

---

## Route: Equality of total logarithms implies R'_Bal(X)=H_X^can
FALSE.

From

exp(sum_X R'_Bal(X))=Xi(F)=exp(sum_gamma H_gamma^can(F))

one may identify the total local function/logarithm under suitable branch/convergence conditions, but a localization-domain decomposition is not unique.

Balaban further says that native R'^(k)(X) is given by series (7.13) [I]. Until that localization formula is recovered, coordinatewise equality is unsupported.

Correction:
Use YM-RPRIME-CANONICAL-C1-1 for the canonical chart and keep YM-RPRIME-NATIVE-COORD-1 open.


---

## Iteration 9 correction: stopping at UNKNOWN after failing to find CMP109 (7.13)
Temporary dead end:
The published CMP109 PDF has no visible Sect. 7 / Eq. (7.13), so a literal search could not recover the formula cited by CMP122-II.

Resolution:
Do not stop at the numbering mismatch. CMP109 explicitly defers localization/cluster expansion to the next paper. Published CMP116 is that continuation. CMP122-II's additional anchor "(7.1) [I]" = first Mayer step matches CMP116 (2.1), and CMP116 (2.13) is the exact-union local connected coefficient. This resolves the legacy manuscript numbering and closes the coordinate dictionary.

---

## Corrected failed assumption: native incompatibility = cube overlap
FALSE.

Published CMP116 Eq. (2.11) makes polymers incompatible when their intersection contains either a cube OR a full wall of a cube.

Consequences:
C_inc=1 is not a valid native anchor factor.
c_stitch=2 is not a valid worst-case native stitching constant.

Correct native sufficient constants in d=4:
C_inc=9,
c_stitch=sqrt(7).
